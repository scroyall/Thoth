namespace Thoth.Transpilation.Tests;

public class PrintTests
    : TranspilerTests
{
    [Test]
    public void Print_DoesNotThrow_WithInteger()
    {
        Program.FakePrintStatement(value: Program.CreateExpression(Type.Integer));

        Transpile();
    }

    // TODO: Add test for string literal.
    // TODO: Add test for boolean.
}