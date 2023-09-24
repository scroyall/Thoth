using Thoth.Parsing;
using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Transpilation.Tests;

public static class Fakes
{
    public static BasicType? UnresolvedType => null;

    public class FakeExpression(BasicType? type)
        : Expression(type);

    public static ParsedProgram Program(params Statement[] statements)
        => new ParsedProgram(statements.ToList(), new List<string>());

    public static SourceReference SourceReference => new SourceReference(-1, -1);

    public static Expression Expression(BasicType? type)
        => new FakeExpression(type);

    public static Expression Boolean
        => Expression(BasicType.Boolean);

#region Statements

    public class FakeStatement()
        : Statement(SourceReference);

    public static FakeStatement Statement => new FakeStatement();

    public static AssertStatement Assert(Expression? condition = null)
        => new AssertStatement(condition ?? Boolean, SourceReference);

    public static AssignmentStatement Assignment(string identifier, BasicType? type)
        => new AssignmentStatement(identifier, Expression(type), SourceReference);

    public static ConditionalStatement Conditional(Expression? condition = null, Statement? statement = null)
        => new ConditionalStatement(condition ?? Boolean, statement ?? Statement, SourceReference);

    public static DefinitionStatement Definition(BasicType? type, string identifier = "fake")
        => new DefinitionStatement(identifier, Expression(type), SourceReference);

    public static PrintStatement Print(BasicType? type)
        => new PrintStatement(Expression(type), SourceReference);

    public static WhileStatement While(Expression? condition = null, Statement? statement = null)
        => new WhileStatement(condition ?? Boolean, statement ?? Statement, SourceReference);

#endregion
}
