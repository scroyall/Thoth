using Thoth.Parsing;

namespace Thoth.Transpilation.Tests;

class ConditionalStatementTests
    : TranspilerTests
{
    [Test]
    public void ConditionalStatement_DoesNotThrow_WhenConditionTypeIsBoolean()
    {
        Transpile(
            Fakes.Conditional(condition: Fakes.Boolean)
        );
    }

    [Test]
    public void ConditionalStatement_ThrowsMismatchedTypeException_WhenConditionTypeIsNotBoolean(
        [Values] BasicType type)
    {
        if (type == BasicType.Boolean) Assert.Ignore("Type is boolean.");

        Assert.Throws<MismatchedTypeException>(() => Transpile(
            Fakes.Conditional(condition: Fakes.Expression(type))
        ));
    }

    [Test]
    public void ConditionalStatement_ThrowsUnresolvedTypeException_WhenConditionTypeIsUnresolved()
    {
        Assert.Throws<UnresolvedTypeException>(() => Transpile(
            Fakes.Conditional(condition: Fakes.Expression(Fakes.UnresolvedType))
        ));
    }
}