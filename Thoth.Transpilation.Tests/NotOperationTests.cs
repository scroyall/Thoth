using Thoth.Parsing.Expressions;

namespace Thoth.Transpilation.Tests;

public class NotOperationTests
    : TranspilerTests
{
    [Test]
    public void NotOperation_WhenValueIsBoolean_Transpiles()
    {
        // TODO: Replace variable definition with a fake expression generator.
        Program.FakeVariableDefinitionStatement(
            Type.Boolean,
            value: new UnaryOperationExpression(
                OperatorType.Not,
                Program.CreateExpression(Type.Boolean)
            )
        );

        Transpile();
    }

    // TODO: Add test for non-boolean value.
}