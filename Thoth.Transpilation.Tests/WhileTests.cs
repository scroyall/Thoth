namespace Thoth.Transpilation.Tests;

class WhileTests
    : TranspilerTests
{
    [Test]
    public void WhileStatement_DoesNotThrow_WhenConditionTypeMatchesBoolean(
        [ResolvedTypes(LowerBound: "bool")] IResolvedType type)
    {
        Program.FakeWhileStatement(condition: Program.CreateExpression(type));

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
}
