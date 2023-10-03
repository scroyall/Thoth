using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

[Parallelizable]
public class PrecedenceTests
    : ParserTests
{
    [Test]
    public void ComparisonOperator_HasPrecedenceOver_FollowingMathematicalOperator(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Comparison  ))] OperatorType comparison,
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Mathematical))] OperatorType mathematical)
    {
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.IdentifierToken();
        Program.OperatorTokens(comparison);
        Program.IdentifierToken();
        Program.OperatorTokens(mathematical);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Exactly(1).TypeOf<AssignmentStatement>()
                                          .And.Property("Value")
                                          .Matches<BinaryOperationExpression>(e => e.Operation == comparison));
    }

    [Test]
    public void ComparisonOperator_HasPrecedenceOver_PrecedingMathematicalOperator(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Comparison  ))] OperatorType comparison,
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Mathematical))] OperatorType mathematical)
    {
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.IdentifierToken();
        Program.OperatorTokens(mathematical);
        Program.IdentifierToken();
        Program.OperatorTokens(comparison);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Exactly(1).TypeOf<AssignmentStatement>()
                                           .And.Property("Value")
                                           .Matches<BinaryOperationExpression>(e => e.Operation == comparison));
    }
}