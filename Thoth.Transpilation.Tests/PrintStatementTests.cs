using Thoth.Parsing;

namespace Thoth.Transpilation.Tests;

public class PrintStatementTests
    : TranspilerTests
{
    [Test]
    public void Print_DoesNotThrow_WithInteger()
    {
        Transpile(Fakes.Program(
            Fakes.Print(BasicType.Integer)
        ));
    }

    [Test]
    public void Print_ThrowsUnresolvedTypeException_WhenExpressionTypeUnresolved()
    {
        Assert.Throws<UnresolvedTypeException>(() => Transpile(
            Fakes.Print(Fakes.UnresolvedType)
        ));
    }
}