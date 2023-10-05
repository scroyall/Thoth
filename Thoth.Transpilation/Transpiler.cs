using Thoth.Parsing;
using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;

namespace Thoth.Transpilation;

public class Transpiler
{
    private ParsedProgram? _program;

    private ParsedProgram Program => _program ?? throw new NullReferenceException();

    private StreamWriter? _writer;

    private StreamWriter Writer => _writer ?? throw new NullReferenceException();

    private int Indents { get; set; }

    /// <summary>
    /// Map of defined variable identifiers to their definitions.
    /// </summary>
    private readonly Dictionary<string, DefinedVariable> _definitions = new();

    /// <summary>
    /// Stack of defined variables.
    /// </summary>
    private readonly Stack<string> _locals = new();

    /// <summary>
    /// Size of the stack.
    /// </summary>
    private int _stackSize;

    /// <summary>
    /// List of scopes by the variable count at creation.
    /// </summary>
    private readonly Stack<int> _scopes = new();

    private readonly Stack<DefinedFunction> _calls = new();

    /// <summary>
    /// Count of labels.
    /// </summary>
    private int _labels;

    public void Transpile(ParsedProgram program, Stream stream)
    {
        _program = program;

        _stackSize = 0;
        _definitions.Clear();
        _scopes.Clear();
        _calls.Clear();
        _labels = 0;

        using (_writer = new StreamWriter(stream))
        {
            WriteLine("global _start");
            WriteLine();

            GenerateDataSection();
            GenerateTextSection();
        }

        _program = null;
    }

    private void GenerateDataSection()
    {
        WriteLine("section .data");
        WriteLine();
        Indent();

        GenerateStrings();

        WriteLine();
        GenerateMacros();

        WriteLine();
        ReserveHeap();

        Outdent();
        WriteLine();
    }

    private void GenerateStrings()
    {
        WriteCommentLine("strings");
        for (int i = 0; i < Program.Strings.Count; i++)
        {
            WriteLine($"string{i}: db `{Program.Strings[i]}`");
            WriteLine($"string{i}_length: equ $ - string{i}");
        }
    }

    private void GenerateMacros()
    {
        WriteLine("%define SYSCALL_WRITE 1");
        WriteLine("%define SYSCALL_EXIT 60");
        WriteLine("%define STDOUT 1");
    }

    private void ReserveHeap()
    {
        WriteLine("heap_start: times 1024 dq 0");
        WriteLine("heap_size_bytes: dq 0");
    }

    private void GenerateTextSection()
    {
        WriteLine("section .text");
        WriteLine();
        Indent();

        WriteCommentLine("functions");
        WriteLine();
        GenerateFunctions();
        WriteLine();

        WriteLabelLine("_start");
        WriteLine();

        OpenStackFrame();
        WriteLine();

        GenerateStatements(Program.Statements);
        GenerateDefaultExit();
        Outdent();
    }

    private void GenerateFunctions()
    {
        foreach (var function in Program.Functions.Values)
        {
            GenerateFunction(function);
        }
    }

    private void GenerateFunction(DefinedFunction function)
    {
        WriteCommentLine(function);

        // Write label to which execution will jump to call the function.
        WriteLabelLine(GetFunctionEntryLabel(function.Name));

        // Adjust the stack size after the call has pushed the call return location on to the stack.
        _stackSize++;

        // Define a hidden parameter for the return value, if the function has one.
        if (function.ReturnType is { } type)
        {
            DefineParameter(type, "_return", function.Parameters.Count);
            _stackSize++;
        }

        // Define any parameters.
        for (int index = 0; index < function.Parameters.Count; index++)
        {
            var parameter = function.Parameters[index];

            DefineParameter(parameter.Type, parameter.Name, function.Parameters.Count - 1 - index);
            _stackSize++;
        }

        OpenStackFrame();

        // Push the function's return type on to the call stack to indicate the valid type to return statements
        // within the function body.
        _calls.Push(function);

        var returned = TryGenerateStatement(function.Body);

        _calls.Pop();

        // Check that a return was guaranteed if the function is expected a return value.
        if (function.ReturnType is { } returnType && !returned) throw new MissingReturnStatementException(returnType);

        WriteLabelLine(GetFunctionExitLabel(function.Name));

        CloseStackFrame();

        // Undefine any parameters.
        foreach (var parameter in function.Parameters)
        {
            UndefineVariable(parameter.Name);
            _stackSize--;
        }

        // Undefine the hidden parameter for the return value, if the function has one.
        if (function.ReturnType is not null)
        {
            UndefineVariable("_return");
            _stackSize--;
        }

        // Return to the call site.
        WriteLine("ret");

        // Adjust the stack size back after the return pops the call return location off the stack.
        _stackSize--;
    }

    private void OpenStackFrame()
    {
        WriteCommentLine("open stack frame");

        // Push the stack base pointer for the previous stack frame onto the stack for later restoration.
        GeneratePush("rbp");

        // Move the current stack pointer into the stack base pointer to create a new stack frame.
        WriteLine("mov rbp, rsp");
    }

    private void CloseStackFrame()
    {
        WriteCommentLine("close stack frame");

        // Move the current stack pointer back to the stack base pointer, just after the previous stack base pointer 
        // which was previously pushed onto the stack.
        WriteLine("mov rsp, rbp");

        // Pop the previous stack base pointer off the stack, restoring the previous stack frame.
        GeneratePop("rbp");
    }

    private void GenerateDefaultExit()
    {
        WriteCommentLine("default exit with code 0");
        WriteLine("mov rdi, 0");
        WriteLine("mov rax, SYSCALL_EXIT");
        WriteLine("syscall");
    }

    #region Statements

    protected virtual bool TryGenerateStatement(Statement statement)
        => GenerateStatement(statement as dynamic);

    protected bool GenerateStatement(Statement statement)
        => throw new UnexpectedStatementException(statement);

    protected bool GenerateStatements(IEnumerable<Statement> statements)
    {
        // Blocks of statements don't guarantee a return by default.
        var returned = false;

        foreach (var statement in statements)
        {
            // Statements following a guaranteed return are unreachable.
            if (returned) throw new UnexpectedStatementException(statement, $"Unexpected unreachable statement {statement}.");

            returned = TryGenerateStatement(statement);
        }

        return returned;
    }

    protected bool GenerateStatement(ReturnStatement statement)
    {
        WriteCommentLine(statement);

        // Not possible to return outside of a call.
        if (_calls.Count == 0) throw new UnexpectedStatementException(statement);

        // Check if we're calling a function which expects a return value.
        if (_calls.Peek().ReturnType is { } type)
        {
            // Return statements must have a return value if the call is expecting one.
            if (statement.Value is null) throw new MissingExpressionException(type);

            WriteCommentLine("return value");

            // Generate the return value, pushing it on to the stack, and check it matches the type expected by the
            // function.
            TryGenerateExpression(statement.Value).Match(type);

            // Pop the return value off the stack.
            GeneratePop("rax");

            // Get the definition for the hidden return parameter.
            var parameter = GetDefinition("_return");

            // Check the type of the hidden return parameter matches the type expected by the function.
            parameter.Type.Match(type);

            // Get the location of the hidden return parameter, and move the return value into it.
            var location = GetVariableLocation(parameter);
            WriteLine($"mov [{location}], rax");
        }
        else if (statement.Value is not null)
        {
            // Returns statements must not have a return value is the call is not expecting one.
            throw new UnexpectedExpressionException(statement.Value);
        }

        var label = GetFunctionExitLabel(_calls.Peek().Name);
        WriteLine($"jmp {label}");

        // Return statements always guarantee a return.
        return true;
    }

    protected bool GenerateStatement(WhileStatement loop)
    {
        WriteCommentLine(loop);

        GenerateConditionalLoop(
            generateTest: (breakLabel) =>
            {
                TryGenerateExpression(loop.Condition).Match(Type.Boolean);

                // Test the result of the condition expression and break out of the loop if it's zero.
                GenerateTestZero(breakLabel);
            },
            generateLoopBody: () => TryGenerateStatement(loop.Body)
        );

        // Even if the loop body guarantees a return, it might not be executed, so while statements can't guarantee
        // a return.
        return false;
    }

    private void GenerateConditionalLoop(Action<string> generateTest, Action generateLoopBody)
    {
        // Generate the label to which execution returns after each loop.
        var conditionLabel = GenerateLabel("_while");

        // Reserve a label to which execution will jump to break the loop.
        var breakLabel = $"{NextLabel}_break";

        // Generate the result of the condition.
        generateTest.Invoke(breakLabel);

        // Generate the loop body.
        generateLoopBody.Invoke();

        // Jump back to the condition expression after the loop body.
        WriteLine($"jmp {conditionLabel}");

        // Write the label to which execution will jump past the loop body to break the loop.
        WriteLine($"{breakLabel}:");
    }

    protected bool GenerateStatement(ConditionalStatement conditional)
    {
        WriteCommentLine(conditional);

        TryGenerateExpression(conditional.Condition).Match(Type.Boolean);

        var label = $"{NextLabel}conditional";
        GenerateTestZero(label);

        TryGenerateStatement(conditional.Body);

        WriteLine($"{label}:");

        // Even if the conditional body guarantees a return, it might not be executed, so conditional statements can't
        // guarantee a return.
        return false;
    }

    protected bool GenerateStatement(AssignmentStatement assignment)
    {
        WriteCommentLine(assignment);

        var definition = GetDefinition(assignment.Identifier);
        TryGenerateExpression(assignment.Value).Match(definition.Type);

        // Pop the result of the expression off the stack.
        GeneratePop("rax");

        // Get the location of the variable and move the result of the expression into it.
        var location = GetVariableLocation(definition);
        WriteLine($"mov QWORD [{location}], rax");

        // Assignments don't generate any statements to guarantee a return.
        return false;
    }

    protected bool GenerateStatement(ScopeStatement scope)
    {
        WriteCommentLine(scope);

        OpenScope();
        var returned = GenerateStatements(scope.Statements);
        CloseScope();

        // Scopes can guarantee a return, so pass it along.
        return returned;
    }

    protected bool GenerateStatement(ExitStatement exit)
    {
        WriteCommentLine(exit);

        TryGenerateExpression(exit.Code);

        WriteCommentLine("exit");

        // Pop exit code from the stack.
        GeneratePop("rdi");

        WriteLine("mov rax, SYSCALL_EXIT");
        WriteLine("syscall");

        // Execution won't continue past this point, but exits guarantee a return to avoid requiring a return statement
        // after an exit anyway.
        return true;
    }

    protected bool GenerateStatement(VariableDefinitionStatement definition)
    {
        WriteCommentLine(definition);

        // Generate the expression for the value, ensuring it's of a resolved type.
        var type = TryGenerateExpression(definition.Value).Match(definition.Type);

        DefineLocalVariable(type, definition.Identifier);

        // Variable definitions don't generate any statements to guarantee a return.
        return false;
    }

    protected bool GenerateStatement(FunctionDefinitionStatement definition)
    {
        WriteCommentLine($"function {definition.Name} definition");

        // Functions don't generate any statements, when defined, to guarantee a return.
        return false;
    }

    protected bool GenerateStatement(IteratorStatement iterator)
    {
        WriteCommentLine(iterator);

        return iterator.Iterable switch
        {
            BinaryOperationExpression { Operation: OperatorType.Range } range => GenerateRangeIterator(iterator, range),
            { } other => GenerateListIterator(iterator)
        };
    }

    protected bool GenerateRangeIterator(IteratorStatement iterator, BinaryOperationExpression range)
    {
        // Open a scope for the iterator's local and anonymous variables.
        OpenScope();

        // Generate the end value.
        TryGenerateExpression(range.Right);

        // Define the end value as an anonymous local variable.
        var last = DefineAnonymousLocalVariable(Type.Integer);

        // Generate the start value.
        TryGenerateExpression(range.Left);

        // Define the current value as a local variable.
        var current = DefineLocalVariable(Type.Integer, iterator.Identifier);

        GenerateConditionalLoop(
            generateTest: (breakLabel) =>
            {
                WriteCommentLine("test iterator");

                // Compare the current value to the last value.
                WriteLine($"mov QWORD rcx, [{GetVariableLocation(current)}]");
                WriteLine($"cmp QWORD rcx, [{GetVariableLocation(last)}]");

                // Break out of the loop if the current value is above the last value.
                WriteLine($"ja {breakLabel}");
            },
            generateLoopBody: () =>
            {
                // Generate the loop body.
                TryGenerateStatement(iterator.Body);

                // Increment the iterator.
                WriteCommentLine($"increment iterator");
                WriteLine($"inc QWORD [{GetVariableLocation(current)}]");
            }
        );

        CloseScope();

        // Even if the loop body guarantees a return, it might not be executed, so loops can't guarantee a return.
        return false;
    }

    protected bool GenerateListIterator(IteratorStatement iterator)
    {
        OpenScope();

        var list = iterator.Iterable switch
        {
            VariableExpression variable => GetDefinition(variable.Identifier),
            ListLiteralExpression literal => DefineAnonymousLocalVariable(TryGenerateExpression(literal)),
            _ => throw new UnexpectedExpressionException(iterator.Iterable),
        };

        // Ensure the iteratable is a list.
        list.Type.MatchList();

        // Get the member type of the list.
        var memberType = list.Type.Parameters[0] ?? throw new NullReferenceException();

        // Push a zero onto the stack for the current index.
        GeneratePush("0");
        var index = DefineAnonymousLocalVariable(Type.Integer);

        // Reserve space on the stack for the current list member.
        GeneratePush(1);
        var current = DefineLocalVariable(memberType, iterator.Identifier);

        GenerateConditionalLoop(
            generateTest: (breakLabel) =>
            {
                // Set the source register to the address of the list in memory, which contains the length.
                WriteLine($"mov QWORD rsi, [{GetVariableLocation(list)}]");

                // Move the current index into the counter register.
                WriteLine($"mov QWORD rcx, [{GetVariableLocation(index)}]");

                // Compare the current index to the length of the list.
                WriteLine($"cmp QWORD rcx, [rsi]");

                // Break the loop if the current index is greater than or equal to the length of the list.
                WriteLine($"jge {breakLabel}");

                // Move the value from the memory location at the index within the list to the current list member.
                WriteLine($"mov rdx, QWORD [rsi + (rcx + 1) * 8]");
                WriteLine($"mov QWORD [{GetVariableLocation(current)}], rdx");
            },
            generateLoopBody: () =>
            {
                TryGenerateStatement(iterator.Body);

                WriteCommentLine($"increment iterator index");
                WriteLine($"inc QWORD [{GetVariableLocation(index)}]");
            }
        );

        CloseScope();

        // Even if the loop body guarantees a return, it might not be executed, so loops can't guarantee a return.
        return false;
    }

    protected bool GenerateStatement(AssertStatement assert)
    {
        WriteCommentLine(assert);

        TryGenerateExpression(assert.Condition).Match(Type.Boolean);

        var label = $"{NextLabel}_assert_pass";

        WriteCommentLine("assert");
        GeneratePop("rax");
        WriteLine("test rax, rax");
        WriteLine($"jnz {label}");

        WriteCommentLine("exit code 134");
        WriteLine("mov rdi, 134");
        WriteLine("mov rax, SYSCALL_EXIT");
        WriteLine("syscall");

        WriteLabelLine(label);

        // Although execution stops after an assert fails, the condition might pass, so asserts can't guarantee a
        // return.
        return false;
    }

    protected bool GenerateStatement(PrintStatement print)
    {
        WriteCommentLine(print);

        switch (print.Value)
        {
            case StringExpression { } literal:
                return GeneratePrintStringLiteral(literal.Index);
            case Expression { } expression:
                // Generate the expression to be printed and ensure the type is resolved.
                var type = TryGenerateExpression(expression);

                if (type.Matches(Type.Integer)) return GeneratePrintSignedInteger();

                throw new NotImplementedException();
        }

        // Prints don't generate any statements to guarantee a return.
        return false;
    }

    private bool GeneratePrintStringLiteral(int index)
    {
        WriteCommentLine("print string");
        WriteLine($"mov rsi, string{index}"); // Set source register to the string.
        WriteLine($"mov rdx, string{index}_length"); // Set data register to the length of the string.
        WriteLine("mov rax, SYSCALL_WRITE");
        WriteLine("mov rdi, STDOUT");
        WriteLine("syscall");

        // Print cannot guarantee a return.
        return false;
    }

    private bool GeneratePrintSignedInteger()
    {
        GenerateLabel("print_signed_integer");
        GeneratePop("rax"); // Pop the signed integer value off the stack.

        WriteCommentLine("capture FLAGS for signed value");
        WriteLine("test rax, rax"); // Set FLAGS for signed value.
        WriteLine("pushf"); // Push FLAGS on to the stack, capturing SF (sign) for later.

        WriteCommentLine("to unsigned value");
        WriteLine("mov rbx, rax"); // Duplicate value for possible restoration.
        WriteLine("neg rax"); // Negate value.
        WriteLine("cmovl rax, rbx"); // Restore copy if negation made the value negative (i.e. it was positive already).

        WriteCommentLine("buffer unsigned value to string");
        WriteLine($"mov rbx, 10"); // Set divisor for base 10.
        // Set source register (buffer pointer) to the top of the stack (the start of the red zone).
        WriteLine("mov rsi, rsp");

        var nextDigit = WriteLocalLabel("next_digit");
        WriteLine("dec rsi"); // Move the buffer pointer one byte into the red zone.
        WriteLine("xor rdx, rdx"); // Zero RDX so RDX:RAX is just RAX.
        WriteLine("div rbx"); // Divide the remaining value by the base.
        WriteLine("add dl, '0'"); // Add ASCII 0 to quotient to get digit character.
        WriteLine("mov [rsi], BYTE dl"); // Move the digit into the buffer in the red zone.
        WriteLine("test rax, rax"); // Check if there's any remainder left.
        WriteLine($"jnz {nextDigit}"); // If so, jump back to buffer the next digit.

        WriteCommentLine("check sign of original value");
        var isPositive = GenerateLocalLabel("is_positive");
        WriteLine("popf"); // Pop FLAGS from the original value off the stack.
        WriteLine($"jns {isPositive}");

        WriteCommentLine("prepend minus sign");
        WriteLine("dec rsi"); // Move the buffer one byte into the red zone.
        WriteLine("mov [rsi], BYTE '-'"); // Move a minus sign into the buffer.

        WriteLabelLine(isPositive);
        WriteCommentLine("print buffer");
        // Set data register to string length by calculating the difference between the start of the string (where the
        // red zone started before the FLAGS were popped) and the buffer pointer.
        WriteLine("lea rdx, [rsp - 8]"); // Subtract 8 because the FLAGS have now been popped off the stack.
        WriteLine("sub rdx, rsi");
        // Source register RSI is already at the start of the string.
        WriteLine("mov rax, SYSCALL_WRITE");
        WriteLine("mov rdi, STDOUT");
        WriteLine("syscall");

        // Print cannot guarantee a return.
        return false;
    }

    protected bool GenerateStatement(FunctionCallStatement call)
    {
        WriteCommentLine(call);

        var type = GenerateFunctionCall(call);
        if (type is not null)
        {
            // Discard the return value from the top of the stack.
            WriteLine("add rsp, 8");
        }

        // Function calls are a separate call, so they can't guarantee a return from the current call.
        return false;
    }

    #endregion

    #region Expressions

    protected virtual Type TryGenerateExpression(Expression expression)
        => GenerateExpression(expression as dynamic);

    protected Type GenerateExpression(Expression expression)
    {
        // Catch any expressions which don't match an overload.
        throw new UnexpectedExpressionException(expression);
    }

    /// <remarks>
    /// Push an integer value on to the stack.
    /// </remarks>
    protected Type GenerateExpression(IntegerExpression integer)
    {
        WriteCommentLine($"integer literal ({integer.Value})");

        // Check if the integer fits in 32 bits, which is the most that can pushed as an immediate.
        if (Math.Abs(integer.Value) < Math.Pow(2, 32))
        {
            GeneratePush($"{integer.Value}");
        }
        else
        {
            // Push integers larger than 32 bits via a register.
            WriteLine($"mov rax, {integer.Value}");
            GeneratePush("rax");
        }

        return Type.Integer;
    }

    protected Type GenerateExpression(FunctionCallExpression call)
    {
        WriteCommentLine(call);

        return GenerateFunctionCall(call) ?? throw new InvalidFunctionException($"Function '{call.Name}' must return a value when used as an expression.");
    }

    protected Type? GenerateFunctionCall(IFunctionCall call)
    {
        // Check that the function has been defined.
        if (!IsFunctionDefined(call.Name)) throw new UndefinedFunctionException(call.Name);

        // Get the function definition.
        var definition = Program.Functions[call.Name] ?? throw new NullReferenceException();

        // Check the number of parameters to the call match the function definition.
        if (call.Parameters.Count != definition.Parameters.Count) throw new InvalidParameterCountException(definition.Parameters.Count, call.Parameters.Count);

        // Reserve a stack entry for the return value, if the function has one, just before the stack frame.
        if (definition.ReturnType is not null)
        {
            WriteCommentLine("return value");
            GeneratePush();
        }

        for (int index = 0; index < definition.Parameters.Count; index++)
        {
            // Generate the expression for the parameter and check the type matches the definition.
            TryGenerateExpression(call.Parameters[index]).Match(definition.Parameters[index].Type);
        }

        // Get the label for the function and execute it.
        var label = GetFunctionEntryLabel(call.Name);
        WriteLine($"call {label}");

        // Discard parameters from the stack, if any.
        if (definition.Parameters.Count > 0)
        {
            WriteCommentLine("discard parameters");
            GeneratePop(definition.Parameters.Count);
        }

        return definition.ReturnType;
    }

    protected Type GenerateExpression(BooleanLiteralExpression boolean)
    {
        WriteCommentLine($"boolean literal ({boolean.Value})");

        GeneratePush(boolean.Value ? "1" : "0");

        return Type.Boolean;
    }

    /// <remarks>
    /// Push a copy of a variable's value on to the stack.
    /// </remarks>
    protected Type GenerateExpression(VariableExpression variable)
    {
        WriteCommentLine(variable);

        var definition = GetDefinition(variable.Identifier);
        var location = GetVariableLocation(definition);
        GeneratePush($"QWORD [{location}]");

        return definition.Type;
    }

    /// <remarks>
    /// Generate the left and right hand sides of a binary expression, perform an operation, and push the result on to
    /// the stack.
    /// </remarks>
    protected Type GenerateExpression(BinaryOperationExpression expression)
    {
        WriteCommentLine(expression);

        // Generate the left and right hand side sub-expressions, pushing their results on to the stack.
        var expressionType = TryGenerateExpression(expression.Left);
        TryGenerateExpression(expression.Right).Match(expressionType);

        // Pop the left and right hand side values from the top of the stack into two registers. 
        GeneratePop("rbx"); // Right hand side.
        GeneratePop("rax"); // Left hand side.

        var operation = expression.Operation;
        if (operation.IsMathemeticalOperation()) return GenerateMathematicalBinaryOperation(operation);
        if (operation.IsComparisonOperation()) return GenerateComparisonOperation(operation);
        if (operation.IsLogicalOperation())
        {
            expressionType.Match(Type.Boolean);

            return GenerateLogicalBinaryOperation(operation);
        }

        throw new UnexpectedExpressionException(expression);
    }

    protected Type GenerateMathematicalBinaryOperation(OperatorType operation)
    {
        switch (operation)
        {
            case OperatorType.Add:
                WriteCommentLine("+");
                WriteLine("add rax, rbx");
                GeneratePush("rax");
                break;
            case OperatorType.Multiply:
                WriteCommentLine("*");
                WriteLine("mul rbx"); // RAX is implied.
                GeneratePush("rax");
                break;
            case OperatorType.Subtract:
                WriteCommentLine("-");
                WriteLine("sub rax, rbx");
                GeneratePush("rax");
                break;
            case OperatorType.Divide:
                WriteCommentLine("/");
                WriteLine("div rbx"); // RAX is implied.
                GeneratePush("rax");
                break;
            default:
                throw new InvalidOperationException(operation, message: $"Expected mathematical operation not {operation}.");
        }

        return Type.Integer;
    }

    protected Type GenerateComparisonOperation(OperatorType operation)
    {
        WriteCommentLine(operation.ToSourceString());

        return operation switch
        {
            OperatorType.GreaterThan        => GenerateComparisonOperation("g"),
            OperatorType.LessThan           => GenerateComparisonOperation("l"),
            OperatorType.GreaterThanOrEqual => GenerateComparisonOperation("ge"),
            OperatorType.LessThanOrEqual    => GenerateComparisonOperation("le"),
            OperatorType.Equal              => GenerateComparisonOperation("e"),
            OperatorType.NotEqual           => GenerateComparisonOperation("ne"),

            _ => throw new InvalidOperationException(operation, message: $"Expected boolean operation not {operation}.")
        };
    }

    protected Type GenerateComparisonOperation(string comparison)
    {
        WriteLine($"xor rcx, rcx"); // Zero out the result register.
        WriteLine($"cmp rax, rbx"); // Compare the two value registers.
        WriteLine($"set{comparison} cl"); // Set the result register to 1 if the comparison is true.
        GeneratePush("rcx"); // Push the result on to the stack.

        return Type.Boolean;
    }

    protected Type GenerateLogicalBinaryOperation(OperatorType operation)
    {
        switch (operation)
        {
            case OperatorType.And:
                WriteCommentLine("and");
                WriteLine("and rax, rbx");
                GeneratePush("rax");
                break;
            case OperatorType.Or:
                WriteCommentLine("or");
                WriteLine("or rax, rbx");
                GeneratePush("rax");
                break;
            default:
                throw new InvalidOperationException(operation, message: $"Expected logical operation not {operation}.");
        }

        return Type.Boolean;
    }

    protected Type GenerateExpression(UnaryOperationExpression expression)
    {
        WriteCommentLine(expression);

        // Generate the sub-expression, pushing its result onto the stack.
        var type = TryGenerateExpression(expression.Value);

        // Pop the sub-expression value from the top of the stack.
        GeneratePop("rax");

        return expression.Operation switch
        {
            OperatorType.Not => GenerateNotOperation(type),

            _ => throw new InvalidOperationException(expression.Operation)
        };
    }

    protected Type GenerateNotOperation(Type type)
    {
        // The not operation can only be applied to boolean expressions.
        type.Match(Type.Boolean);

        WriteCommentLine("not");

        // Invert the value.
        WriteLine("xor rax, 1");

        // Push the inverted value back on to the stack.
        GeneratePush("rax");

        // The not operation always 
        return Type.Boolean;
    }

    protected Type GenerateExpression(ListLiteralExpression list)
    {
        WriteCommentLine(list);

        // Allocate heap memory for the list length and any values.
        // Destination register RDI now holds the starting address of the allocated memory.
        GenerateAllocation((list.Values.Count + 1) * 8);

        // Push the starting address of the allocated memory on to the stack as the generated value.
        GeneratePush("rdi");

        // Write the length of the list to the first word.
        WriteLine($"mov QWORD [rdi], {list.Values.Count}");

        var memberType = list.MemberType;
        foreach (var value in list.Values)
        {
            // Advance the destination register to the memory allocated for the next value in the list.
            WriteLine("add rdi, 8");

            // Save the destination register in case generating the value expression overwrites it.
            // This occurs if the value is itself a list literal.
            GeneratePush("rdi");

            // Generate the value expression and check the type matches the member type of the list.
            memberType = TryGenerateExpression(value).Match(memberType);

            // Pop the generate value off the stack in a temporary register.
            GeneratePop("rax");

            // Restore the previously saved destination register.
            GeneratePop("rdi");

            // Move the generated value out of the temporary register into the heap memory allocated to it.
            WriteLine("mov QWORD [rdi], rax");
        }

        return Type.List(memberType);
    }

    protected Type GenerateExpression(IndexExpression index)
    {
        WriteCommentLine(index);

        // Generate the expression for the indexable, which is the location of the start of the list in memory.
        var indexableType = TryGenerateExpression(index.Indexable);
        if (indexableType.Root != BuiltinType.List) throw new MismatchedTypeException(BuiltinType.List, indexableType);

        // Generate the expression for the index, and check it's an integer.
        TryGenerateExpression(index.Index).Match(Type.Integer);

        // Pop the index into the data register.
        GeneratePop("rdx");

        // Pop the memory location of the list off the stack into the source register.
        GeneratePop("rsi");

        // Push the value from the memory location within the list on to the stack for the expression result.
        WriteCommentLine("index into list");
        GeneratePush("QWORD [rsi + (rdx + 1) * 8]");

        return indexableType.Parameters[0];
    }

    #endregion

    #region Scopes

    /// <summary>
    /// Open a new scope.
    /// </summary>
    /// <remarks>
    /// Records the variable count so that any variables defined while inside the scope can be undefined once the scope
    /// is closed.
    /// </remarks>
    private void OpenScope()
    {
        _scopes.Push(_locals.Count);

        var number = _scopes.Count;
        WriteCommentLine($"open scope {number}");
    }

    private void CloseScope()
    {
        WriteCommentLine($"close scope {_scopes.Count}");

        // Calculate the difference between the current number of variables and the number when the scope was opened.
        var delta = _locals.Count - _scopes.Pop();

        // Check we aren't trying to  
        if (delta > _locals.Count) throw new Exception("Variable count is less than scope target!");

        for (int i = 0; i < delta; i++)
        {
            UndefineLocal();
        }

        _stackSize -= delta;

        // Move the stack pointer over all the discarded variables.
        WriteLine($"add rsp, {delta} * 8");
    }

    #endregion

    #region Stack Operations

    /// <summary>
    /// Generate the pushing of an entry on to the stack. 
    /// </summary>
    protected void GeneratePush(string source)
    {
        _stackSize++;
        WriteLine($"push {source}");
    }

    protected void GeneratePush(int count = 1)
    {
        _stackSize += count;
        WriteLine($"sub rsp, {count} * 8");
    }

    protected void GeneratePop(string destination)
    {
        if (_stackSize <= 0) throw new Exception("Stack is empty.");

        _stackSize--;
        WriteLine($"pop {destination}");
    }

    protected void GeneratePop(int count = 1)
    {
        if (_stackSize < count) throw new Exception("Stack is empty.");

        _stackSize -= count;
        WriteLine($"add rsp, {count} * 8");
    }

    private void GeneratePeek(string destination, int offset = 0)
    {
        if (_stackSize <= 0) throw new Exception("Stack is empty.");

        WriteLine($"mov {destination}, QWORD [rsp + {offset} * 8]");
    }

    #endregion

#region Variables

    /// <summary>
    /// Define a variable.
    /// </summary>
    /// <param name="identifier">Unique identifier for the variable.</param>
    /// <exception cref="MultiplyDefinedVariableException">If the variable is already defined.</exception>
    private DefinedVariable DefineLocalVariable(Type type, string identifier)
    {
        if (IsDefined(identifier)) throw new MultiplyDefinedVariableException(identifier);

        // Add the identifier to the stack of locally defined variables.
        _locals.Push(identifier);

        // Add the position of the variable on the stack to the map of variable stack indices.
        var definition = new DefinedVariable(VariableScope.Local, type, _stackSize);
        _definitions[identifier] = definition;

        return definition;
    }

    private DefinedVariable DefineAnonymousLocalVariable(Type type)
    {
        // Push an empty string on to the stack of locally defined variables.
        _locals.Push(string.Empty);

        return new(VariableScope.Local, type, _stackSize);
    }

    private void DefineParameter(Type type, string identifier, int offset)
    {
        if (IsDefined(identifier)) throw new MultiplyDefinedFunctionException(identifier);

        _definitions[identifier] = new(VariableScope.Parameter, type, offset);
    }

    private void UndefineVariable(string identifier)
    {
        if (!IsDefined(identifier)) throw new UndefinedFunctionException(identifier);

        _definitions.Remove(identifier);
    }

    /// <summary>
    /// Pop a variable off the stack of defined variables.
    /// </summary>
    private void UndefineLocal()
    {
        // Pop the top identifier off the stack of defined variables.
        var identifier = _locals.Pop();

        // Remove non-anonymous definitions from the map of variable definitions by identifier.
        if (identifier != string.Empty)
        {
            UndefineVariable(identifier);
        }
    }

    /// <summary>
    /// Pop a number of variables off the stack of defined variables.
    /// </summary>
    private void UndefineLocals(int count)
    {
        for (int index = 0; index < count; index++)
        {
            UndefineLocal();
        }
    }

    /// <summary>
    /// Check if a variable is defined.
    /// </summary>
    /// <param name="identifier">Unique identifier for the variable.</param>
    private bool IsDefined(string identifier)
    {
        return _definitions.ContainsKey(identifier);
    }

    private DefinedVariable GetDefinition(string identifier)
    {
        if (!IsDefined(identifier)) throw new UndefinedVariableException(identifier);

        return _definitions[identifier];
    }

    private string GetVariableLocation(VariableScope scope, int offset)
    {
        return scope switch
        {
            VariableScope.Local => $"rbp - {offset - 1} * 8",
            VariableScope.Parameter => $"rbp + ({offset} + 2) * 8",
            _ => throw new NotImplementedException()
        };
    }

    private string GetVariableLocation(DefinedVariable definition)
        => GetVariableLocation(definition.Scope, definition.Offset);

#endregion

#region Labels

    // TODO(SR 210923) Don't implement this as a property because it increments when inspected in the debugger.
    private string NextLabel => $"label{++_labels}";

    private string GenerateLabel(string suffix = "")
    {
        var label = $"{NextLabel}_{suffix}";
        WriteLabelLine(label);
        return label;
    }

    private string GenerateLocalLabel(string label)
    {
        return $".{label}";
    }

    private string WriteLocalLabel(string label)
    {
        label = GenerateLocalLabel(label);
        WriteLabelLine(label);
        return label;
    }

#endregion

#region Tests

    /// <summary>
    /// Generate a test that the value on the top of the stack value is zero.
    /// Execution jumps to the label if the value is zero..
    /// </summary>
    private void GenerateTestZero(string label, bool preserve = false)
    {
        // Pop the value to test off the stack.
        GeneratePop("rax");

        // Jump to the label if the value is zero.
        WriteLine("test rax, rax");
        WriteLine($"jz {label}");
    }

    /// <summary>
    /// Generate a test that the value on the top of the stack is greater than the value underneath it.
    /// Execution jumps to the label if the value is greater.
    /// </summary>
    /// <param name="label">Label to which execution jumps.</param>
    /// <param name="preserve">Keep the values on the stack?</param>
    private void GenerateTestGreater(string label, bool preserve = false)
        => GenerateTest("ja", label, preserve);

    /// <summary>
    /// Generate a comparison of the top two values on the stack.
    /// </summary>
    /// <param name="instruction">Comparison instruction. e.g. je, ja, jb etc.</param>
    /// <param name="label">Label to which execution jumps.</param>
    /// <param name="preserve"></param>
    private void GenerateTest(string instruction, string label, bool preserve = false)
    {
        if (!preserve)
        {
            // Pop the values off the stack into a pair of registers.
            GeneratePop("rax");
            GeneratePop("rbx");
        }
        else
        {
            // Leave the values on the stack by peeking them instead.
            GeneratePeek("rax", 0);
            GeneratePeek("rbx", 1);
        }

        WriteLine("cmp rax, rbx");
        WriteLine($"{instruction} {label}");
    }

#endregion

#region Writing

    private void WriteLine(string text = "")
    {
        for (int i = 0; i < Indents; i++)
        {
            Writer.Write('\t');
        }

        Writer.WriteLine(text);
    }

    protected void WriteCommentLine(string text = "")
    {
        WriteLine($"; {text}");
    }

    private void WriteCommentLine(object obj)
        => WriteCommentLine(obj.ToString() ?? throw new NullReferenceException());

    private void WriteLabelLine(string label)
    {
        Writer.WriteLine($"{label}:");
    }

    #endregion

    #region Indentation

    private void Indent()
    {
        Indents++;
    }

    private void Outdent()
    {
        if (Indents < 1) throw new Exception("Not indented!");

        Indents--;
    }

    #endregion

    #region Functions

    private string GetFunctionEntryLabel(string name)
        => $"function_{name}_entry";

    private string GetFunctionExitLabel(string name)
        => $"function_{name}_exit";

    private bool IsFunctionDefined(string name)
        => Program.Functions.ContainsKey(name);

#endregion

#region Heap

    private void GenerateAllocation(int bytes)
    {
        WriteCommentLine($"allocate {bytes} bytes");

        WriteLine("mov QWORD rdi, [heap_size_bytes]");
        WriteLine("add rdi, heap_start");

        WriteLine($"add QWORD [heap_size_bytes], {bytes}");
    }

#endregion
}
