namespace Thoth.Transpilation.Tests;

public class VariableDefinitionTests
    : TranspilerTests
{
    [Test]
    public void DefinitionStatement_Transpiles_WhenExpressionTypeEqualsVariableType([Values] Type type)
    {
        Program.FakeVariableDefinitionStatement(type);

        Transpile();
    }

    [Test]
    public void DefinitionStatement_ThrowsMismatchedTypeException_WhenExpressionTypeDoesNotMatchVariableType(
        [Values] Type variableType,
        [Values] Type expressionType)
    {
        if (expressionType.Matches(variableType)) Assert.Ignore("Expression type matches variable type.");

        Program.FakeVariableDefinitionStatement(type: variableType, value: Program.CreateExpression(expressionType));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}