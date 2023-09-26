using Thoth.Parsing;
using Thoth.Parsing.Expressions;

namespace Thoth.Transpilation.Tests;

public class NotOperationTests
    : TranspilerTests
{
    [Test]
    public void Transpiles_NotOperation_WithBooleanLiteral([Values] bool value)
    {
        Transpile(
            Fakes.Definition(null,
                expression: new UnaryOperationExpression(BasicType.Boolean, OperatorType.Not,
                    value: new BooleanLiteralExpression(value)))
        );
    }

    [Test]
    public void Transpiles_NotOperation_WithBooleanExpression()
    {
        Transpile(
            Fakes.Definition(null,
                expression: new UnaryOperationExpression(BasicType.Boolean, OperatorType.Not,
                    value: Fakes.Expression(BasicType.Boolean)))
        );
    }
}