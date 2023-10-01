namespace Thoth.Transpilation.Tests;

class AssertTests
    : TranspilerTests
{
    [Test]
    public void AssertStatement_WhenConditionTypeMatchesBoolean_Transpiles(
        [ResolvedTypes(LowerBound: "bool")] IResolvedType type)
    {
        Program.FakeAssertStatement(condition: Program.CreateExpression(type));

        Transpile();
    }

    [Test]
    public void AssertStatement_WhenConditionTypeDoesNotMatchBoolean_ThrowsException(
        [ResolvedTypes(UpperBound: "bool")] IResolvedType type)
    {
        Program.FakeAssertStatement(condition: Program.CreateExpression(type));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}
