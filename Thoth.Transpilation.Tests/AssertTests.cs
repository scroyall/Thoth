namespace Thoth.Transpilation.Tests;

class AssertTests
    : TranspilerTests
{
    [Test]
    public void AssertStatement_WhenConditionTypeMatchesBoolean_Transpiles()
    {
        Program.FakeAssertStatement(condition: Program.CreateExpression(Type.Boolean));

        Transpile();
    }

    [Test]
    public void AssertStatement_WhenConditionTypeDoesNotMatchBoolean_ThrowsException([Types(Except: "bool")] Type type)
    {
        Program.FakeAssertStatement(condition: Program.CreateExpression(type));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}
