namespace Thoth.Transpilation.Tests;

class WhileTests
    : TranspilerTests
{
    [Test]
    public void WhileStatement_WhenConditionIsBoolean_Transpiles()
    {
        Program.FakeWhileStatement(condition: Program.CreateExpression(Type.Boolean));

        Transpile();
    }

    [Test]
    public void WhileStatement_WhenConditionIsNotBoolean_ThrowsException([Types(Except: "bool")] Type type)
    {
        Program.FakeWhileStatement(condition: Program.CreateExpression(type));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}
