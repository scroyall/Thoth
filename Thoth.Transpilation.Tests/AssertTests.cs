namespace Thoth.Transpilation.Tests;

class AssertTests
    : TranspilerTests
{
    [Test]
    public void AssertStatement_DoesNotThrow_WhenConditionTypeIsBoolean()
    {
        Program.FakeAssertStatement(condition: Program.CreateExpression(BasicType.Boolean));

        Transpile();
    }

    [Test]
    public void AssertStatement_ThrowsMismatchedTypeException_WhenConditionTypeIsNotBoolean(
        [Values] BasicType type)
    {
        if (type == BasicType.Boolean) Assert.Ignore("Type is boolean.");

        Program.FakeAssertStatement(condition: Program.CreateExpression(type));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }

    [Test]
    public void AssertStatement_ThrowsUnresolvedTypeException_WhenConditionTypeIsUnresolved()
    {
        Program.FakeAssertStatement(condition: Program.CreateUnresolvedExpression());

        Assert.Throws<UnresolvedTypeException>(Transpile);
    }
}