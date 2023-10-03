namespace Thoth.Transpilation.Tests;

public class ReturnTests
    : TranspilerTests
{
    [Test]
    public void Return_ThrowsException_WhenNotInCall()
    {
        Program.FakeReturnStatement();

        Assert.Throws<UnexpectedStatementException>(Transpile);
    }

    [Test]
    public void ReturnWithoutValue_Transpiles_WhenInCallWithoutReturnType()
    {
        Program.FakeFunctionDefinitionStatement(
            returnType: null,
            body: Program.CreateReturnStatement()
        );

        Transpile();
    }

    [Test]
    public void ReturnWithoutValue_ThrowsException_WhenInCallWithReturnType(
        [Types] Type type)
    {
        Program.FakeFunctionDefinitionStatement(
            returnType: type,
            body: Program.CreateReturnStatement()
        );

        Assert.Throws<MissingExpressionException>(Transpile);
    }

    [Test]
    public void ReturnWithValue_Transpiles_WhenInCallWithMatchingReturnType(
        [Types] Type type)
    {
        Program.FakeFunctionDefinitionStatement(
            returnType: type,
            body: Program.CreateReturnStatement(Program.CreateExpression(type))
        );

        Transpile();
    }

    [Test]
    public void ReturnWithValue_ThrowsException_WhenInCallWithMismatchedReturnType(
        [Types] Type returnType,
        [Types] Type valueType)
    {
        if (valueType.Matches(returnType)) Assert.Ignore("Value type matches return type.");

        Program.FakeFunctionDefinitionStatement(
            returnType: returnType,
            body: Program.CreateReturnStatement(Program.CreateExpression(valueType))
        );

        Assert.Throws<MismatchedTypeException>(Transpile);
    }

    [Test]
    public void Return_ThrowsException_WithUnreachableStatements()
    {
        Program.FakeFunctionDefinitionStatement(
            body: Program.FakeScopeStatement(
                [
                    Program.CreateReturnStatement(),
                    Program.FakeStatement() 
                ]
            )
        );

        Assert.Throws<UnexpectedStatementException>(Transpile);
    }
}