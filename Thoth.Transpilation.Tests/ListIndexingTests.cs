namespace Thoth.Transpilation.Tests;

public class ListIndexTests
    : TranspilerTests
{
    [Test]
    public void ListIndexing_WithIntegerLiteralIndex_Transpiles([Types] Type memberType)
    {
        // Define a list variable to index.
        var listDefinition = Program.FakeVariableDefinitionStatement(
            type: Type.List(memberType),
            value: Program.CreateListLiteralExpression(memberType)
        );

        // Index the list variable with an integer literal.
        Program.FakeVariableDefinitionStatement(
            type: memberType,
            value: Program.CreateIndexExpression(
                indexable: Program.CreateVariableExpression(listDefinition.Identifier),
                index: Program.CreateIntegerLiteralExpression()
            )
        );

        Transpile();
    }
}
