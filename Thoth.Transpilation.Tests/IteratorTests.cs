namespace Thoth.Transpilation.Tests;

public class IteratorTests
    : TranspilerTests
{
    [Test]
    public void Iterator_WithRange_Transpiles()
    {
        Program.FakeIteratorStatement(
            iterable: Program.CreateRangeExpression()
        );

        Transpile();
    }

    [Test]
    public void Iterator_WithListLiteral_Transpiles([Types] Type type)
    {
        Program.FakeIteratorStatement(
            iterable: Program.CreateListLiteralExpression(type)
        );

        Transpile();
    }

    [Test]
    public void Iterator_WithListVariable_Transpiles([Types] Type memberType)
    {
        var list = Program.FakeVariableDefinitionStatement(Type.List(memberType));

        Program.FakeIteratorStatement(
            iterable: Program.CreateVariableExpression(list.Identifier)
        );

        Transpile();
    }
}
