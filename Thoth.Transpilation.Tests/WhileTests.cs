namespace Thoth.Transpilation.Tests;

class WhileTests
    : TranspilerTests
{
    [Test]
    public void WhileStatement_DoesNotThrow_WhenConditionTypeIsBoolean()
    {
        Program.FakeWhileStatement(condition: Program.CreateExpression(BasicType.Boolean));

        Transpile();
    }

    [Test]
    public void WhileStatement_ThrowsMismatchedTypeException_WhenConditionTypeIsNotBoolean(
        [Values] BasicType type)
    {
        if (type == BasicType.Boolean) Assert.Ignore("Type is boolean.");

        Program.FakeWhileStatement(condition: Program.CreateExpression(type));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }

    [Test]
    public void WhileStatement_ThrowsUnresolvedTypeException_WhenConditionTypeIsUnresolved()
    {
        Program.FakeWhileStatement(condition: Program.CreateUnresolvedExpression());

        Assert.Throws<UnresolvedTypeException>(Transpile);
    }
}