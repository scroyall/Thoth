using Thoth.Parsing;
using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Transpilation.Tests;

public class FakeExpression(BasicType? type)
    : Expression(type);

public record FakeStatement()
    : Statement(new SourceReference(-1, -1));

public class FakeParsedProgram
{
    public List<Statement> Statements { get; } = [];
    public List<string> Strings { get; } = [];
    public Dictionary<string, DefinedFunction> Functions { get; } = [];

    public static readonly SourceReference FakeSourceReference = new(-1, -1);

    public int NameCount = 0;

    public DefinedFunction FakeDefinedFunction(string? name = null)
    {
        var defined = new DefinedFunction(
            name ?? $"function{++NameCount}",
            FakeStatements(),
            FakeSourceReference
        );

        if (Functions.Keys.Contains(defined.Name)) throw new MultiplyDefinedFunctionException(defined.Name);
        Functions[defined.Name] = defined;

        return defined;
    }

    public ParsedProgram ToParsedProgram()
        => new(Statements, Strings, Functions);

#region Statements

    public TStatement AddStatement<TStatement>(TStatement statement)
        where TStatement : Statement
    {
        Statements.Add(statement);
        return statement;
    }

    public FakeStatement FakeStatement()
        => new();

    public List<FakeStatement> FakeStatements(int count = 3)
        => Enumerable.Repeat(FakeStatement(), count).ToList();

    public AssertStatement FakeAssertStatement(Expression? condition = null)
        => AddStatement(
            new AssertStatement(
                condition ?? FakeExpression(BasicType.Boolean),
                FakeSourceReference
            )
        );

    public AssignmentStatement FakeAssignmentStatement(string identifier, Expression? value = null)
        => AddStatement(
            new AssignmentStatement(
                identifier,
                value ?? FakeUnresolvedExpression(),
                FakeSourceReference
            )
        );

    public ConditionalStatement FakeConditionalStatement(Expression? condition = null, Statement? statement = null)
        => AddStatement(
            new ConditionalStatement(
                condition ?? FakeExpression(BasicType.Boolean),
                statement ?? FakeStatement(),
                FakeSourceReference
            )
        );

    public FunctionCallStatement FakeFunctionCallStatement(string? name = null)
        => AddStatement(
            new FunctionCallStatement(
                name ?? FakeDefinedFunction().Name,
                FakeSourceReference
            )
        );
    
    public FunctionCallStatement FakeUndefinedFunctionCallStatement()
        => AddStatement(
            new FunctionCallStatement(
                $"undefinedfunction{++NameCount}",
                FakeSourceReference
            )
        );

    public FunctionDefinitionStatement FakeFunctionDefinitionStatement(string? name = null)
        => AddStatement(
            new FunctionDefinitionStatement(
                FakeDefinedFunction(name).Name, FakeSourceReference
            )
        );

    public PrintStatement FakePrintStatement(Expression? value = null)
        => AddStatement(
            new PrintStatement(
                value ?? FakeExpression(BasicType.String),
                FakeSourceReference
            )
        );

    public VariableDefinitionStatement FakeVariableDefinitionStatement(BasicType? type = null, string? name = null, Expression? value = null)
        => AddStatement(
            new VariableDefinitionStatement(
                type,
                name ?? $"variable{++NameCount}",
                value ?? FakeExpression(type),
                FakeSourceReference
            )
        );

    public WhileStatement FakeWhileStatement(Expression? condition = null, Statement? body = null)
        => AddStatement(
            new WhileStatement(
                condition ?? FakeExpression(BasicType.Boolean),
                body ?? FakeStatement(),
                FakeSourceReference
            )
        );

#endregion

#region Expressions

    public FakeExpression FakeExpression(BasicType? type)
        => new(type);

    public FakeExpression FakeUnresolvedExpression()
        => new(null);

#endregion
}