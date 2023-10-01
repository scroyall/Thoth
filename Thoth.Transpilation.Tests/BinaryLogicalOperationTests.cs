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
            value: new BinaryOperationExpression(
                BasicType.Boolean,
                operation,
                Left: Program.CreateExpression(BasicType.Boolean),
                Right: Program.CreateExpression(BasicType.Boolean)
            )
        );

        Transpile();
    }
    
    [Test]
    public void BinaryLogicalOperation_WhenOperandDoesNotMatchBoolean_ThrowsException(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation,
        [ResolvedTypes()] IResolvedType leftType,
        [ResolvedTypes()] IResolvedType rightType)
    {
        if (leftType.Matches(BasicType.Boolean) && rightType.Matches(BasicType.Boolean)) Assert.Ignore("Operands both match boolean.");

        // TODO: Replace variable definition with a fake expression generator.
        Program.FakeVariableDefinitionStatement(
            value: new BinaryOperationExpression(
                BasicType.Boolean,
                operation,
                Left: Program.CreateExpression(leftType),
                Right: Program.CreateExpression(rightType)
            )
        );

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}