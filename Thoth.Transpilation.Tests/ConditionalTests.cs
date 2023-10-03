namespace Thoth.Transpilation.Tests;

class ConditionalTests
    : TranspilerTests
{
    [Test]
    public void ConditionalStatement_WhenConditionIsBoolean_Transpiles()
    {
        Program.FakeConditionalStatement(condition: Program.CreateExpression(Type.Boolean));

        Transpile();
    }

    [Test]
    public void ConditionalStatement_WhenConditionIsNotBoolean_ThrowsException([Types(Except: "bool")] Type type)
    {
        Program.FakeConditionalStatement(condition: Program.CreateExpression(type));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}
