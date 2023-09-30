namespace Thoth.Transpilation.Tests;

public class PrintTests
    : TranspilerTests
{
    [Test]
    public void Print_DoesNotThrow_WithInteger()
    {
        Program.FakePrintStatement(value: Program.CreateExpression(BasicType.Integer));

        Transpile();
    }

    // TODO: Add test for string literal.
    // TODO: Add test for boolean.

    [Test]
    public void Print_ThrowsUnresolvedTypeException_WhenExpressionTypeIsUnresolved()
    {
        Program.FakePrintStatement(value: Program.CreateUnresolvedExpression());

        Assert.Throws<UnresolvedTypeException>(Transpile);
    }
}