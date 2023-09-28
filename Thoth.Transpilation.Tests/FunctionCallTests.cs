namespace Thoth.Transpilation.Tests;

public class FunctionCallTests
    : TranspilerTests
{
    [Test]
    public void FunctionCall_Transpiles_WhenFunctionDefined()
    {
        var definition = Program.FakeFunctionDefinitionStatement();

        Program.FakeFunctionCallStatement(definition.Name);

        Transpile();
    }

    [Test]
    public void FunctionCall_ThrowsException_WhenFunctionNotDefined()
    {
        Program.FakeUndefinedFunctionCallStatement();

        Assert.Throws<UndefinedFunctionException>(Transpile);
    }
}