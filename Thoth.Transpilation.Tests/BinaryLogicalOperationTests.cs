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
        // TODO: Replace variable definition with a fake expression generator.
        Program.FakeVariableDefinitionStatement(
            value: new BinaryOperationExpression(
                BasicType.Boolean,
                operation,
                left: Program.FakeExpression(BasicType.Boolean),
                right: Program.FakeExpression(BasicType.Boolean)
            )
        );

        Transpile();
    }

    [Test]
    public void BinaryLogicalOperation_ThrowsException_WhenValuesAreUnresolved(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        // TODO: Replace variable definition with a fake expression generator.
        Program.FakeVariableDefinitionStatement(
            value: new BinaryOperationExpression(
                BasicType.Boolean,
                operation,
                left: Program.FakeUnresolvedExpression(),
                right: Program.FakeUnresolvedExpression()
            )
        );

        Assert.Throws<UnresolvedTypeException>(Transpile);
    }
    
    [Test]
    public void BinaryLogicalOperation_ThrowsException_WhenValuesAreNotBoolean(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation,
        [Values] BasicType leftType,
        [Values] BasicType rightType)
    {
        if (leftType == BasicType.Boolean && rightType == BasicType.Boolean) Assert.Ignore("Values are both boolean.");

        // TODO: Replace variable definition with a fake expression generator.
        Program.FakeVariableDefinitionStatement(
            value: new BinaryOperationExpression(
                BasicType.Boolean,
                operation,
                left: Program.FakeExpression(leftType),
                right: Program.FakeExpression(rightType)
            )
        );

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}