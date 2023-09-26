using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tests;

namespace Thoth.Parsing.Tests;

[Parallelizable]
public class PrecedenceTests
    : ParserTests
{
    [Test]
    public void BinaryBooleanOperator_HasPrecedenceOver_FollowingMathematicalOperator(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryBoolean))] OperatorType boolean,
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Mathematical ))] OperatorType mathematical)
    {
        var program = Parse($"var value = 0 {boolean.ToSourceString()} 0 {mathematical.ToSourceString()} 0;");

        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .And.Property("Value")
                                           .Matches<BinaryOperationExpression>(e => e.Operation == boolean));
    }

    [Test]
    public void BinaryBooleanOperator_HasPrecedenceOver_PrecedingMathematicalOperator(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryBoolean))] OperatorType boolean,
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.Mathematical ))] OperatorType mathematical)
    {
        var program = Parse($"var value = 0 {mathematical.ToSourceString()} 0 {boolean.ToSourceString()} 0;");

        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .And.Property("Value")
                                           .Matches<BinaryOperationExpression>(e => e.Operation == boolean));
    }
}