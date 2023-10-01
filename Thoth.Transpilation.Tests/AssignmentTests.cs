namespace Thoth.Transpilation.Tests;

public class AssignmentTests
    : TranspilerTests
{
    [Test]
    public void Assignment_WhenExpressionTypeMatchesVariable_Transpiles(
        [ResolvedTypes()] IResolvedType variableType,
        [ResolvedTypes()] IResolvedType expressionType)
    {
        if (!expressionType.Matches(variableType)) Assert.Ignore("Expression type does not match variable.");

        var definition = Program.FakeVariableDefinitionStatement(type: variableType);

        Program.FakeAssignmentStatement(definition.Identifier, Program.CreateExpression(expressionType));

        Transpile();
    }

    [Test]
    public void Assignment_WhenExpressionTypeDoesNotMatchVariable_ThrowsException(
        [ResolvedTypes()] IResolvedType variableType,
        [ResolvedTypes()] IResolvedType expressionType)
    {
        if (expressionType.Matches(variableType)) Assert.Ignore("Expression type matches variable.");

        var definition = Program.FakeVariableDefinitionStatement(type: variableType);

        Program.FakeAssignmentStatement(definition.Identifier, Program.CreateExpression(expressionType));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}