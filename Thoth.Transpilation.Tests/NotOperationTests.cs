using Thoth.Parsing;
using Thoth.Parsing.Expressions;

namespace Thoth.Transpilation.Tests;

public class NotOperationTests
    : TranspilerTests
{
    [Test]
    public void Transpiles_NotOperation_WhenValueIsBoolean()
    {
        Transpile(
            Fakes.Definition(null,
                expression: new UnaryOperationExpression(BasicType.Boolean, OperatorType.Not,
                    value: Fakes.Boolean))
        );
    }
}