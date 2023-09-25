using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;

namespace Thoth.Parsing.Tests;

[Parallelizable]
public class PrecedenceTests
    : ParserTests
{
    [Test]
    public void BooleanOperator_HasPrecedenceOver_FollowingMathematicalOperator(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Boolean     ))] OperatorType boolean,
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Mathematical))] OperatorType mathematical)
    {
        var program = Parse($"var value = 0 {boolean.ToSourceString()} 0 {mathematical.ToSourceString()} 0;");

        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .And.Property("Value")
                                           .Matches<BinaryOperationExpression>(e => e.Operation == boolean));
    }

    [Test]
    public void BooleanOperator_HasPrecedenceOver_PrecedingMathematicalOperator(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Boolean     ))] OperatorType boolean,
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Mathematical))] OperatorType mathematical)
    {
        var program = Parse($"var value = 0 {mathematical.ToSourceString()} 0 {boolean.ToSourceString()} 0;");

        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .And.Property("Value")
                                           .Matches<BinaryOperationExpression>(e => e.Operation == boolean));
    }
}