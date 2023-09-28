namespace Thoth.Transpilation.Tests;

public class AssignmentTests
    : TranspilerTests
{
    [Test]
    public void Assignment_DoesNotThrow_WhenExpressionTypeMatchesDefinition(
        [Values] BasicType type)
    {
        var definition = Program.FakeVariableDefinitionStatement(type: type);

        Program.FakeAssignmentStatement(definition.Identifier, Program.FakeExpression(type));

        Transpile();
    }

    [Test]
    public void Assignment_ThrowsUnresolvedTypeException_WhenExpressionTypeIsUnresolved(
        [Values] BasicType variableType)
    {
        var definition = Program.FakeVariableDefinitionStatement(type: variableType);

        Program.FakeAssignmentStatement(definition.Identifier, Program.FakeUnresolvedExpression());

        Assert.Throws<UnresolvedTypeException>(Transpile);
    }

    [Test]
    public void Assignment_ThrowsMismatchedTypeException_WhenExpressionTypeDiffersFromDefinition(
        [Values] BasicType variableType,
        [Values] BasicType expressionType)
    {
        if (expressionType == variableType) Assert.Ignore("Expression type matches variable type.");

        var definition = Program.FakeVariableDefinitionStatement(type: variableType);

        Program.FakeAssignmentStatement(definition.Identifier, Program.FakeExpression(expressionType));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}