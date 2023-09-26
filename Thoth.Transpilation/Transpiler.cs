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
    private readonly Dictionary<string, DefinedVariable> _variableDefinitions = new();

    /// <summary>
    /// Stack of defined variables.
    /// </summary>
    private readonly Stack<string> _variables = new();

    /// <summary>
    /// Size of the stack.
    /// </summary>
    private int _stackSize;

    /// <summary>
    /// List of scopes by the variable count at creation.
    /// </summary>
    private readonly Stack<int> _scopes = new();

    /// <summary>
    /// Count of labels.
    /// </summary>
    private int _labels;

    public void Transpile(ParsedProgram program, Stream stream)
    {
        _program = program;

        _stackSize = 0;
        _variableDefinitions.Clear();
        _scopes.Clear();
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

    private void GenerateTextSection()
    {
        WriteLine("section .text");
        WriteLine();

        WriteLabelLine("_start");

        Indent();
        GenerateStatements(Program.Statements);
        GenerateDefaultExit();
        Outdent();
    }

    private void GenerateDefaultExit()
    {
        WriteCommentLine("default exit with code 0");
        WriteLine("mov rdi, 0");
        WriteLine("mov rax, SYSCALL_EXIT");
        WriteLine("syscall");
    }

    #region Statements

    protected virtual void TryGenerateStatement(Statement statement)
        => GenerateStatement(statement as dynamic);

    protected void GenerateStatement(Statement statement)
        => throw new UnexpectedStatementException(statement);

    protected void GenerateStatements(IEnumerable<Statement> statements)
    {
        foreach (var statement in statements)
        {
            TryGenerateStatement(statement);
        }
    }

    protected void GenerateStatement(WhileStatement loop)
    {
        WriteCommentLine(loop);

        GenerateConditionalLoop(
            generateTest: (breakLabel) =>
            {
                TryGenerateExpression(loop.Condition).CheckMatches(BasicType.Boolean);

                // Test the result of the condition expression and break out of the loop if it's zero.
                GenerateTestZero(breakLabel);
            },
            generateLoopBody: () => TryGenerateStatement(loop.Statement)
        );
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

    protected void GenerateStatement(ConditionalStatement conditional)
    {
        WriteCommentLine(conditional);

        TryGenerateExpression(conditional.Condition).CheckMatches(BasicType.Boolean);

        var label = $"{NextLabel}conditional";
        GenerateTestZero(label);

        TryGenerateStatement(conditional.Statement);

        WriteLine($"{label}:");
    }

    protected void GenerateStatement(AssignmentStatement assignment)
    {
        WriteCommentLine(assignment);

        var definition = GetVariableDefinition(assignment.Identifier);
        TryGenerateExpression(assignment.Value).CheckMatches(definition.Type);

        // Pop the result of the expression off the stack.
        GeneratePop("rax");

        // Move the result of the expression into the variable on the stack.
        var stackOffset = GetVariableStackOffset(assignment.Identifier);
        WriteLine($"mov [rsp + {stackOffset} * 8], rax");
    }

    protected void GenerateStatement(ScopeStatement scope)
    {
        WriteCommentLine(scope);

        OpenScope();
        GenerateStatements(scope.Statements);
        CloseScope();
    }

    protected void GenerateStatement(ExitStatement exit)
    {
        WriteCommentLine(exit);

        TryGenerateExpression(exit.Code);

        WriteCommentLine("exit");

        // Pop exit code from the stack.
        GeneratePop("rdi");

        WriteLine("mov rax, SYSCALL_EXIT");
        WriteLine("syscall");
    }

    protected void GenerateStatement(DefinitionStatement definition)
    {
        WriteCommentLine(definition);

        // Generate the expression for the value, ensuring it's of a resolved type.
        var expressionType = TryGenerateExpression(definition.Value);

        // Check the expression type matches the definition type.
        expressionType.CheckMatches(definition.Type);

        PushDefinedVariable(definition.Type ?? expressionType, definition.Identifier);
    }

    protected void GenerateStatement(EnumeratorStatement enumerator)
    {
        WriteCommentLine(enumerator);

        OpenScope();

        // Generate the end value.
        TryGenerateExpression( enumerator.Range.End);

        // Generate the start value.
        TryGenerateExpression( enumerator.Range.Start);

        // Initialize the variable holding the enumerator's current value with the start value.
        PushDefinedVariable(BasicType.Integer, enumerator.Identifier);

        GenerateConditionalLoop(
            generateTest: (breakLabel) =>
            {
                // Test if the current value is greater than the end value, and break out of the loop if it is.
                GenerateTestGreater(breakLabel, preserve: true);
            },
            generateLoopBody: () =>
            {
                // Generate the loop body.
                TryGenerateStatement(enumerator.Body);

                // Increment the enumerator.
                GeneratePop("rcx");
                WriteLine("inc rcx");
                GeneratePush("rcx");
            }
        );

        CloseScope();
    }

    protected void GenerateStatement(AssertStatement assert)
    {
        WriteCommentLine(assert);

        TryGenerateExpression(assert.Condition).CheckMatches(BasicType.Boolean);

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
    }

    protected void GenerateStatement(PrintStatement print)
    {
        WriteCommentLine(print);

        switch (print.Expression)
        {
            case StringExpression { } literal:
                GeneratePrintStringLiteral(literal.Index);
                break;
            case Expression { } expression:
                var type = TryGenerateExpression(expression);

                switch (type)
                {
                    case BasicType.Integer:
                        GeneratePrintSignedInteger();
                        break;
                    default:
                        throw new NotImplementedException();
                }
                break;
        }
    }

    private void GeneratePrintStringLiteral(int index)
    {
        WriteCommentLine("print string");
        WriteLine($"mov rsi, string{index}"); // Set source register to the string.
        WriteLine($"mov rdx, string{index}_length"); // Set data register to the length of the string.
        WriteLine("mov rax, SYSCALL_WRITE");
        WriteLine("mov rdi, STDOUT");
        WriteLine("syscall");
    }

    private void GeneratePrintSignedInteger()
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
    }

    #endregion

    #region Expressions

    protected virtual BasicType TryGenerateExpression(Expression expression)
        => GenerateExpression(expression as dynamic);

    protected BasicType GenerateExpression(Expression expression)
    {
        // Catch any expressions which don't match an overload.
        throw new UnexpectedExpressionException(expression);
    }

    /// <remarks>
    /// Push an integer value on to the stack.
    /// </remarks>
    protected BasicType GenerateExpression(IntegerExpression integer)
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

        return BasicType.Integer;
    }

    protected BasicType GenerateExpression(BooleanLiteralExpression boolean)
    {
        WriteCommentLine($"boolean literal ({boolean.Value})");

        GeneratePush(boolean.Value ? "1" : "0");

        return BasicType.Boolean;
    }

    /// <remarks>
    /// Push a copy of a variable's value on to the stack.
    /// </remarks>
    protected BasicType GenerateExpression(VariableExpression variable)
    {
        WriteCommentLine(variable);

        var definition = GetVariableDefinition(variable.Identifier);
        var stackOffset = GetVariableStackOffset(variable.Identifier);
        GeneratePush($"QWORD[rsp + {stackOffset} * 8]");

        return definition.Type;
    }

    /// <remarks>
    /// Generate the left and right hand sides of a binary expression, perform an operation, and push the result on to
    /// the stack.
    /// </remarks>
    protected BasicType GenerateExpression(BinaryOperationExpression expression)
    {
        WriteCommentLine(expression);

        // Generate the left and right hand side sub-expressions, pushing their results on to the stack.
        var expressionType = TryGenerateExpression(expression.Left);
        TryGenerateExpression(expression.Right).CheckMatches(expressionType);

        // Pop the left and right hand side values from the top of the stack into two registers. 
        GeneratePop("rbx"); // Right hand side.
        GeneratePop("rax"); // Left hand side.

        var operation = expression.Operation;
        if (operation.IsMathemeticalOperation()) return GenerateMathematicalBinaryOperation(operation);
        if (operation.IsBooleanOperation()) return GenerateBooleanBinaryOperation(operation);
        if (operation.IsLogicalOperation())
        {
            expressionType.CheckMatches(BasicType.Boolean);

            return GenerateLogicalBinaryOperation(operation);
        }

        throw new UnexpectedExpressionException(expression);
    }

    protected BasicType GenerateMathematicalBinaryOperation(OperatorType operation)
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

        return BasicType.Integer;
    }

    protected BasicType GenerateBooleanBinaryOperation(OperatorType operation)
    {
        WriteCommentLine(operation.ToSourceString());

        return operation switch
        {
            OperatorType.GreaterThan        => GenerateBooleanBinaryOperation("g"),
            OperatorType.LessThan           => GenerateBooleanBinaryOperation("l"),
            OperatorType.GreaterThanOrEqual => GenerateBooleanBinaryOperation("ge"),
            OperatorType.LessThanOrEqual    => GenerateBooleanBinaryOperation("le"),
            OperatorType.Equal              => GenerateBooleanBinaryOperation("e"),
            OperatorType.NotEqual           => GenerateBooleanBinaryOperation("ne"),

            _ => throw new InvalidOperationException(operation, message: $"Expected boolean operation not {operation}.")
        };
    }

    protected BasicType GenerateBooleanBinaryOperation(string comparison)
    {
        WriteLine($"xor rcx, rcx"); // Zero out the result register.
        WriteLine($"cmp rax, rbx"); // Compare the two value registers.
        WriteLine($"set{comparison} cl"); // Set the result register to 1 if the comparison is true.
        GeneratePush("rcx"); // Push the result on to the stack.

        return BasicType.Boolean;
    }

    protected BasicType GenerateLogicalBinaryOperation(OperatorType operation)
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

        return BasicType.Boolean;
    }

    protected BasicType GenerateExpression(UnaryOperationExpression expression)
    {
        WriteCommentLine(expression);

        // Generate the sub-expression, pushing its result onto the stack.
        TryGenerateExpression(expression.Value).CheckMatches(expression.Type);

        // Pop the sub-expression value from the top of the stack.
        GeneratePop("rax");

        return expression.Operation switch
        {
            OperatorType.Not => GenerateNotOperation(expression),

            _ => throw new InvalidOperationException(expression.Operation)
        };
    }

    protected BasicType GenerateNotOperation(UnaryOperationExpression expression)
    {
        // The not operation can only be applied to boolean expressions.
        expression.Type.CheckMatches(BasicType.Boolean);

        WriteCommentLine("not");

        // Invert the value.
        WriteLine("xor rax, 1");

        // Push the inverted value back on to the stack.
        GeneratePush("rax");

        // The not operation always 
        return BasicType.Boolean;
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
        _scopes.Push(_variables.Count);

        var number = _scopes.Count;
        WriteCommentLine($"open scope {number}");
    }

    private void CloseScope()
    {
        WriteCommentLine($"close scope {_scopes.Count}");

        // Calculate the difference between the current number of variables and the number when the scope was opened.
        var delta = _variables.Count - _scopes.Pop();

        // Check we aren't trying to  
        if (delta > _variables.Count) throw new Exception("Variable count is less than scope target!");

        for (int i = 0; i < delta; i++)
        {
            PopDefinedVariable();
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

    private void GeneratePop(string destination)
    {
        if (_stackSize <= 0) throw new Exception("Stack is empty.");

        _stackSize--;
        WriteLine($"pop {destination}");
    }

    private void GeneratePeek(string destination, int offset = 0)
    {
        if (_stackSize <= 0) throw new Exception("Stack is empty.");

        WriteLine($"mov {destination}, QWORD [rsp + {offset} * 8]");
    }

    #endregion

    #region Variables

    /// <summary>
    /// Push a variable on to the stack of defined variables.
    /// </summary>
    /// <param name="identifier">Unique identifier for the variable.</param>
    /// <exception cref="MultiplyDefinedVariableException">If the variable is already defined.</exception>
    private void PushDefinedVariable(BasicType type, string identifier)
    {
        if (IsVariableDefined(identifier)) throw new MultiplyDefinedVariableException(identifier);

        // Add the identifier to the stack of defined variables.
        _variables.Push(identifier);

        // Add the position of the variable on the stack to the map of variable stack indices.
        _variableDefinitions[identifier] = new DefinedVariable(type, _stackSize);
    }

    /// <summary>
    /// Pop a variable off the stack of defined variables.
    /// </summary>
    private void PopDefinedVariable()
    {
        // Pop the top identifier off the stack of defined variables.
        var identifier = _variables.Pop();

        // Remove the position of the variable on the stack from the map of variable stack indices.
        _variableDefinitions.Remove(identifier);
    }

    /// <summary>
    /// Check if a variable is defined.
    /// </summary>
    /// <param name="identifier">Unique identifier for the variable.</param>
    private bool IsVariableDefined(string identifier)
    {
        return _variables.Contains(identifier);
    }

    /// <summary>
    /// Get the offset of a defined variable from the current top of the stack.
    /// </summary>
    /// <param name="identifier">Unique identifier for the variable.</param>
    /// <returns></returns>
    /// <exception cref="UndefinedVariableException">If the variable is not defined.</exception>
    private int GetVariableStackOffset(string identifier)
    {
        if (!_variableDefinitions.TryGetValue(identifier, out var definition))
        {
            throw new UndefinedVariableException(identifier);
        }

        return _stackSize - definition.Index;
    }

    private DefinedVariable GetVariableDefinition(string identifier)
    {
        if (!IsVariableDefined(identifier)) throw new UndefinedVariableException(identifier);

        return _variableDefinitions[identifier];
    }

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
}