using Thoth.Parsing;

namespace Thoth.Transpilation.Tests;

class AssertStatementTests
    : TranspilerTests
{
    [Test]
    public void AssertStatement_DoesNotThrow_WhenConditionTypeIsBoolean()
    {
        Transpile(
            Fakes.Assert(condition: Fakes.Boolean)
        );
    }

    [Test]
    public void AssertStatement_ThrowsMismatchedTypeException_WhenConditionTypeIsNotBoolean(
        [Values] BasicType type)
    {
        if (type == BasicType.Boolean) Assert.Ignore("Type is boolean.");

        Assert.Throws<MismatchedTypeException>(() => Transpile(
            Fakes.Assert(condition: Fakes.Expression(type))
        ));
    }

    [Test]
    public void AssertStatement_ThrowsUnresolvedTypeException_WhenConditionTypeIsUnresolved()
    {
        Assert.Throws<UnresolvedTypeException>(() => Transpile(
            Fakes.Assert(condition: Fakes.Expression(Fakes.UnresolvedType))
        ));
    }
}