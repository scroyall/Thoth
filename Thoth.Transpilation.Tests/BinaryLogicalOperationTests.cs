using Thoth.Parsing.Expressions;
using Thoth.Tests;

namespace Thoth.Transpilation.Tests;

public class BinaryLogicalOperationTests
    : TranspilerTests
{
    [Test]
    public void BinaryLogicalOperation_WhenOperandsMatchBoolean_Transpiles(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        // TODO: Replace variable definition with a fake expression generator.
        Program.FakeVariableDefinitionStatement(
            Type.Boolean,
            value: new BinaryOperationExpression(
                operation,
                Left: Program.CreateExpression(Type.Boolean),
                Right: Program.CreateExpression(Type.Boolean)
            )
        );

        Transpile();
    }
    
    [Test]
    public void BinaryLogicalOperation_WhenOperandDoesNotMatchBoolean_ThrowsException(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation,
        [Types] Type leftType,
        [Types] Type rightType)
    {
        if (leftType.Matches(Type.Boolean) && rightType.Matches(Type.Boolean)) Assert.Ignore("Operands both match boolean.");

        // TODO: Replace variable definition with a fake expression generator.
        Program.FakeVariableDefinitionStatement(
            Type.Boolean,
            value: new BinaryOperationExpression(
                operation,
                Left: Program.CreateExpression(leftType),
                Right: Program.CreateExpression(rightType)
            )
        );

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}
