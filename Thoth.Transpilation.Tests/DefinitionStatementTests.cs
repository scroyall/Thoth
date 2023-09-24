using Thoth.Parsing;

namespace Thoth.Transpilation.Tests;

public class DefinitionStatementTests
    : TranspilerTests
{
    [Test]
    public void Definition_DoesNotThrow_WithType(
        [ValueSource(nameof(Types))] BasicType type)
    {
        Transpile(
            Fakes.Definition(type)
        );
    }

    [Test]
    public void Definition_ThrowsUnresolvedTypeException_WhenTypeIsUnresolved()
    {
        Assert.Throws<UnresolvedTypeException>(() => Transpile(
            Fakes.Definition(Fakes.UnresolvedType)
        ));
    }
}