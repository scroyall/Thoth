namespace Thoth.Transpilation.Tests;

public class ListExpressionTests
    : TranspilerTests
{
    [Test]
    public void ListExpression_WithNoMembers_Transpiles([Types] Type type)
    {
        Program.FakeVariableDefinitionStatement(
            type: Type.List(type),
            value: Program.CreateListLiteralExpression(type, count: 0)
        );

        Transpile();
    }

    [Test]
    public void ListExpression_WithOneMemberOfMatchingType_Transpiles([Types] Type type)
    {
        Program.FakeVariableDefinitionStatement(
            type: Type.List(type),
            value: Program.CreateListLiteralExpression(type, count: 1)
        );

        Transpile();
    }

    [Test]
    public void ListExpression_WithMultipleMembersOfMatchingType_Transpiles([Types] Type type)
    {
        Program.FakeVariableDefinitionStatement(
            type: Type.List(type),
            value: Program.CreateListLiteralExpression(type, count: 3)
        );

        Transpile();
    }

    [Test]
    public void ListExpression_WithMembersOfMismatchedType_ThrowsException([Types] Type desiredType, [Types] Type actualType)
    {
        if (actualType.Matches(desiredType)) Assert.Ignore("Actual type matches desired type.");

        Program.FakeVariableDefinitionStatement(
            type: Type.List(desiredType),
            value: Program.CreateListLiteralExpression(actualType)
        );

        Assert.Throws<MismatchedTypeException>(Transpile);
    }
}
