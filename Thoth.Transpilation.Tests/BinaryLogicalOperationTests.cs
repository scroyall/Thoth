using Thoth.Parsing.Expressions;
using Thoth.Tests;

namespace Thoth.Transpilation.Tests;

public class BinaryLogicalOperationTests
    : TranspilerTests
{
    [Test]
    public void BinaryLogicalOperation_Transpiles_WhenValuesAreBoolean(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        Transpile(
            Fakes.Definition(null,
                expression: new BinaryOperationExpression(BasicType.Boolean, operation, left: Fakes.Boolean, right: Fakes.Boolean)));
    }

    [Test]
    public void BinaryLogicalOperation_ThrowsException_WhenValuesAreUnresolved(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        Assert.Throws<UnresolvedTypeException>(() => Transpile(
            Fakes.Definition(null,
                expression: new BinaryOperationExpression(BasicType.Boolean, operation,
                    left: Fakes.Expression(Fakes.UnresolvedType),
                    right: Fakes.Expression(Fakes.UnresolvedType)))));
    }
    
    [Test]
    public void BinaryLogicalOperation_ThrowsException_WhenValuesAreNotBoolean(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation,
        [Values] BasicType leftType,
        [Values] BasicType rightType)
    {
        if (leftType == BasicType.Boolean && rightType == BasicType.Boolean) Assert.Ignore("Values are both boolean.");

        Assert.Throws<MismatchedTypeException>(() => Transpile(
            Fakes.Definition(null,
                expression: new BinaryOperationExpression(BasicType.Boolean, operation,
                    left: Fakes.Expression(leftType),
                    right: Fakes.Expression(rightType)))));
    }
}