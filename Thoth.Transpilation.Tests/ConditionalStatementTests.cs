using Thoth.Parsing;

namespace Thoth.Transpilation.Tests;

class ConditionalStatementTests
    : TranspilerTests
{
    [Test]
    public void ConditionalStatement_DoesNotThrow_WhenConditionReturnsBoolean()
    {
        Transpile(
            Fakes.Conditional(condition: Fakes.Boolean)
        );
    }

    [Test]
    public void ConditionalStatement_ThrowsMismatchedTypeException_WhenConditionReturnsNonBooleanResolvedType(
        [ValueSource(nameof(Types))] BasicType type)
    {
        if (type == BasicType.Boolean) Assert.Ignore("Type is boolean.");

        Assert.Throws<MismatchedTypeException>(() => Transpile(
            Fakes.Conditional(condition: Fakes.Expression(type))
        ));
    }

    [Test]
    public void ConditionalStatement_ThrowsUnresolvedTypeException_WhenConditionReturnsUnresolvedType()
    {
        Assert.Throws<UnresolvedTypeException>(() => Transpile(
            Fakes.Conditional(condition: Fakes.Expression(Fakes.UnresolvedType))
        ));
    }
}