using Thoth.Parsing.Expressions;

namespace Thoth.Transpilation.Tests;

// TODO: Rename to BooleanLiteralExpressionTests.
public class LiteralExpressionTests
    : TranspilerTests
{
    [Test]
    public void Transpiles_BooleanLiteral_WithTrueValue()
    {
        // TODO: Replace variable definition with a fake expression generator.
        Program.FakeVariableDefinitionStatement(value: new BooleanLiteralExpression(true));

        Transpile();
    }

    // TODO: Add test for false value.
}