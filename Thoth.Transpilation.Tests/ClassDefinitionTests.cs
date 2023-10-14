namespace Thoth.Transpilation.Tests;

public class ClassDefinitionTests
    : TranspilerTests
{
    [Test]
    public void ClassDefinition_WithoutMembers_Transpiles()
    {
        Program.FakeClassDefinitionStatement();

        Transpile();
    }
}
