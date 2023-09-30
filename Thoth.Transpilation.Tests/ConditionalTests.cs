namespace Thoth.Transpilation.Tests;

class ConditionalTests
    : TranspilerTests
{
    [Test]
    public void ConditionalStatement_DoesNotThrow_WhenConditionTypeIsBoolean()
    {
        Program.FakeConditionalStatement(condition: Program.CreateExpression(BasicType.Boolean));

        Transpile();
    }

    [Test]
    public void ConditionalStatement_ThrowsMismatchedTypeException_WhenConditionTypeIsNotBoolean(
        [Values] BasicType type)
    {
        if (type == BasicType.Boolean) Assert.Ignore("Type is boolean.");

        Program.FakeConditionalStatement(condition: Program.CreateExpression(type));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }

    [Test]
    public void ConditionalStatement_ThrowsUnresolvedTypeException_WhenConditionTypeIsUnresolved()
    {
        Program.FakeConditionalStatement(condition: Program.CreateUnresolvedExpression());

        Assert.Throws<UnresolvedTypeException>(Transpile);
    }
}