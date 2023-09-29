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
            statements: [ Program.GenerateReturnStatement() ]
        );

        Transpile();
    }

    [Test]
    public void ReturnWithoutValue_ThrowsException_WhenInCallWithReturnType([Values] BasicType type)
    {
        Program.FakeFunctionDefinitionStatement(
            returnType: type,
            statements: [ Program.GenerateReturnStatement() ]
        );

        Assert.Throws<MissingExpressionException>(Transpile);
    }

    [Test]
    public void ReturnWithValue_Transpiles_WhenInCallWithMatchingReturnType([Values] BasicType type)
    {
        Program.FakeFunctionDefinitionStatement(
            returnType: type,
            statements: [Program.GenerateReturnStatement(Program.FakeExpression(type))]
        );

        Transpile();
    }

    [Test]
    public void ReturnWithValue_ThrowsException_WhenInCallWithMismatchedReturnType([Values] BasicType returnType, [Values] BasicType valueType)
    {
        if (valueType.Matches(returnType)) Assert.Ignore("Value type matches return type.");

        Program.FakeFunctionDefinitionStatement(
            returnType: returnType,
            statements: [Program.GenerateReturnStatement(Program.FakeExpression(valueType))]
        );

        Assert.Throws<MismatchedTypeException>(Transpile);
    }

    [Test]
    public void Return_ThrowsException_WithUnreachableStatements()
    {
        Program.FakeFunctionDefinitionStatement(
            statements: [Program.GenerateReturnStatement(), new FakeStatement()]
        );

        Assert.Throws<UnexpectedStatementException>(Transpile);
    }
}