using Thoth.Parsing;
using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Transpilation.Tests;

public record FakeExpression(IType type)
    : Expression(type);

public record FakeStatement()
    : Statement(new SourceReference(-1, -1));

public record FakeExpressionGeneratorStatement(Expression Expression)
    : Statement(new SourceReference(-1, -1));

public class FakeParsedProgram
{
    public List<Statement> Statements { get; } = [];
    public List<string> Strings { get; } = [];
    public Dictionary<string, DefinedFunction> Functions { get; } = [];

    public static readonly SourceReference FakeSourceReference = new(-1, -1);

    public int NameCount = 0;

    public DefinedFunction FakeDefinedFunction(string? name = null, List<NamedParameter>? parameters = null, IResolvedType? returnType = null, Statement? body = null)
    {
        var defined = new DefinedFunction(
            name ?? $"function{++NameCount}",
            parameters ?? [],
            returnType,
            body ?? FakeStatement(),
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

    public List<Statement> FakeStatements(int count = 3)
        => Enumerable.Repeat(FakeStatement() as Statement, count).ToList();

    public AssertStatement FakeAssertStatement(Expression? condition = null)
        => AddStatement(
            new AssertStatement(
                condition ?? CreateExpression(BasicType.Boolean),
                FakeSourceReference
            )
        );

    public AssignmentStatement FakeAssignmentStatement(string identifier, Expression? value = null)
        => AddStatement(
            new AssignmentStatement(
                identifier,
                value ?? CreateExpression(BasicType.Integer),
                FakeSourceReference
            )
        );

    public ConditionalStatement FakeConditionalStatement(Expression? condition = null, Statement? statement = null)
        => AddStatement(
            new ConditionalStatement(
                condition ?? CreateExpression(BasicType.Boolean),
                statement ?? FakeStatement(),
                FakeSourceReference
            )
        );

    public FakeExpressionGeneratorStatement CreateExpressionGeneratorStatement(Expression expression)
        => new(expression);

    public FunctionCallStatement FakeFunctionCallStatement(string? name = null)
        => AddStatement(
            new FunctionCallStatement(
                name ?? FakeDefinedFunction().Name,
                [],
                FakeSourceReference
            )
        );
    
    public FunctionCallStatement FakeUndefinedFunctionCallStatement()
        => AddStatement(
            new FunctionCallStatement(
                $"undefinedfunction{++NameCount}",
                [],
                FakeSourceReference
            )
        );

    public FunctionDefinitionStatement FakeFunctionDefinitionStatement(string? name = null, List<NamedParameter>? parameters = null, IResolvedType? returnType = null, Statement? body = null)
        => AddStatement(
            new FunctionDefinitionStatement(
                FakeDefinedFunction(name, parameters, returnType, body).Name, FakeSourceReference
            )
        );

    public NamedParameter CreateNamedParameter(IResolvedType type, string? name = null)
        => new(type, name ?? $"parameter{++NameCount}");

    public PrintStatement FakePrintStatement(Expression? value = null)
        => AddStatement(
            new PrintStatement(
                value ?? CreateExpression(BasicType.String),
                FakeSourceReference
            )
        );

    public ReturnStatement FakeReturnStatement(Expression? value = null)
        => AddStatement(CreateReturnStatement(value));

    public ReturnStatement CreateReturnStatement(Expression? value = null)
        => new ReturnStatement(value, FakeSourceReference);

    public ScopeStatement FakeScopeStatement(List<Statement>? body = null)
        => new((body ?? []).ToList(), FakeSourceReference);

    public VariableDefinitionStatement FakeVariableDefinitionStatement(IType? type = null, string? name = null, Expression? value = null)
        => AddStatement(CreateVariableDefinitionStatement(type, name, value));

    public VariableDefinitionStatement CreateVariableDefinitionStatement(IType? type = null, string? name = null, Expression? value = null)
        => new(
            type ?? BasicType.Unresolved,
            name ?? $"variable{++NameCount}",
            value ?? CreateExpression(type ?? BasicType.Unresolved),
            FakeSourceReference
        );

    public WhileStatement FakeWhileStatement(Expression? condition = null, Statement? body = null)
        => AddStatement(
            new WhileStatement(
                condition ?? CreateExpression(BasicType.Boolean),
                body ?? FakeStatement(),
                FakeSourceReference
            )
        );

#endregion

#region Expressions

    public FakeExpression CreateExpression(IType type)
        => new(type);

    public VariableExpression CreateVariableExpression(string? identifier = null)
        => new(identifier ?? $"variable{++NameCount}");

#endregion
}