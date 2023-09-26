using Thoth.Parsing.Expressions;

namespace Thoth.Transpilation.Tests;

public class LiteralExpressionTests
    : TranspilerTests
{
    [Test]
    public void Transpiles_BooleanLiteral_WithTrueValue()
    {
        Transpile(
            Fakes.Definition(null, expression: new BooleanLiteralExpression(true))
        );
    }
}