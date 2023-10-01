namespace Thoth.Transpilation.Tests;

class ConditionalTests
    : TranspilerTests
{
    [Test]
    public void ConditionalStatement_WhenConditionTypeMatchesBoolean_Transpiles(
        [ResolvedTypes(LowerBound: "bool")] IResolvedType type)
    {
        Program.FakeConditionalStatement(condition: Program.CreateExpression(type));

        Transpile();
    }

    [Test]
    public void ConditionalStatement_WhenConditionTypeDoesNotMatchBoolean_ThrowsException(
        [ResolvedTypes(UpperBound: "bool")] IResolvedType type)
    {
        Program.FakeConditionalStatement(condition: Program.CreateExpression(type));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}