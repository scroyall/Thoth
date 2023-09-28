namespace Thoth.Transpilation.Tests;

public class FunctionDefinitionTests
    : TranspilerTests
{
    [Test]
    public void FunctionDefinitionStatement_Transpiles()
    {
        Program.FakeFunctionDefinitionStatement();

        Transpile();
    }
}