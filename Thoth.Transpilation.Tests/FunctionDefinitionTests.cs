namespace Thoth.Transpilation.Tests;

public class FunctionDefinitionTests
    : TranspilerTests
{
    [Test]
    public void FunctionDefinition_Transpiles_WithoutReturnType()
    {
        Program.FakeFunctionDefinitionStatement(returnType: null);

        Transpile();
    }

    [Test]
    public void FunctionDefinitionWithReturnType_Transpiles_WithReturnStatement([Values] BasicType type)
    {
        var returnStatement = Program.GenerateReturnStatement(
            value: Program.FakeExpression(type));

        Program.FakeFunctionDefinitionStatement(
            returnType: type,
            statements: [ returnStatement ]);

        Transpile();
    }

    [Test]
    public void FunctionDefinitionWithReturnType_ThrowsException_WithoutReturnStatement([Values] BasicType type)
    {
        Program.FakeFunctionDefinitionStatement(
            returnType: type,
            statements: []);

        Assert.Throws<MissingReturnStatementException>(Transpile);
    }
}