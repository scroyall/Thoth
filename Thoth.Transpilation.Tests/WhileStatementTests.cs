using Thoth.Parsing;

namespace Thoth.Transpilation.Tests;

class WhileStatementTests
    : TranspilerTests
{
    [Test]
    public void WhileStatement_DoesNotThrow_WhenConditionTypeIsBoolean()
    {
        Transpile(
            Fakes.While(condition: Fakes.Boolean)
        );
    }

    [Test]
    public void WhileStatement_ThrowsMismatchedTypeException_WhenConditionTypeIsNotBoolean(
        [ValueSource(nameof(Types))] BasicType type)
    {
        if (type == BasicType.Boolean) Assert.Ignore("Type is boolean.");

        Assert.Throws<MismatchedTypeException>(() => Transpile(
            Fakes.While(condition: Fakes.Expression(type))
        ));
    }

    [Test]
    public void WhileStatement_ThrowsUnresolvedTypeException_WhenConditionTypeIsUnresolved()
    {
        Assert.Throws<UnresolvedTypeException>(() => Transpile(
            Fakes.While(condition: Fakes.Expression(Fakes.UnresolvedType))
        ));
    }
}