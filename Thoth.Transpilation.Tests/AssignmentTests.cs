namespace Thoth.Transpilation.Tests;

public class AssignmentTests
    : TranspilerTests
{
    [Test]
    public void Assignment_WhenExpressionTypeMatchesVariable_Transpiles([Types] Type type)
    {
        var definition = Program.FakeVariableDefinitionStatement(type: type);

        Program.FakeAssignmentStatement(definition.Identifier, Program.CreateExpression(type));

        Transpile();
    }

    [Test]
    public void Assignment_WhenExpressionTypeDoesNotMatchVariable_ThrowsException([Types] Type variableType, [Types] Type expressionType)
    {
        if (expressionType.Matches(variableType)) Assert.Ignore("Expression type matches variable.");

        var definition = Program.FakeVariableDefinitionStatement(type: variableType);

        Program.FakeAssignmentStatement(definition.Identifier, Program.CreateExpression(expressionType));

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}
