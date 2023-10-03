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
    public void FunctionDefinitionWithReturnType_Transpiles_WithReturnStatement([Types] Type type)
    {
        var returnStatement = Program.CreateReturnStatement(
            value: Program.CreateExpression(type));

        Program.FakeFunctionDefinitionStatement(
            returnType: type,
            body: returnStatement);

        Transpile();
    }

    [Test]
    public void FunctionDefinitionWithReturnType_ThrowsException_WithoutReturnStatement([Types] Type type)
    {
        Program.FakeFunctionDefinitionStatement(
            returnType: type,
            body: Program.FakeStatement());

        Assert.Throws<MissingReturnStatementException>(Transpile);
    }

    [Test]
    public void FunctionDefinitionWithParameter_Transpiles_WithReferenceToParameter([Types] Type type)
    {
        var parameter = Program.CreateNamedParameter(type: type);

        Program.FakeFunctionDefinitionStatement(
            parameters: [ parameter ],
            body: Program.CreateExpressionGeneratorStatement(
                Program.CreateVariableExpression(parameter.Name)
            )
        );

        Transpile();
    }

    [Test]
    public void FunctionDefinitionWithParameter_Throws_WithReferenceToVariableDefinedOutsideFunction([Types] Type type)
    {
        var definition = Program.FakeVariableDefinitionStatement(type);

        Program.FakeFunctionDefinitionStatement(
            body: Program.CreateExpressionGeneratorStatement(
                Program.CreateVariableExpression(definition.Identifier)
            )
        );

        Assert.Throws<UndefinedVariableException>(Transpile);
    }

    [Test]
    public void FunctionDefinitionWithParameter_Transpiles_WithReferenceToVariableDefinedInsideFunction([Types] Type type)
    {
        var definition = Program.CreateVariableDefinitionStatement(type: type);

        Program.FakeFunctionDefinitionStatement(
            body: Program.FakeScopeStatement([
                definition,
                Program.CreateExpressionGeneratorStatement(
                    Program.CreateVariableExpression(definition.Identifier)
                )
            ])
        );

        Transpile();
    }
}
