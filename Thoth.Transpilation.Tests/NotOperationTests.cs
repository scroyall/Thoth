using Thoth.Parsing.Expressions;

namespace Thoth.Transpilation.Tests;

public class NotOperationTests
    : TranspilerTests
{
    [Test]
    public void Transpiles_NotOperation_WhenValueTypeMatchesBoolean(
        [ResolvedTypes(LowerBound: "bool")] IResolvedType type)
    {
        // TODO: Replace variable definition with a fake expression generator.
        Program.FakeVariableDefinitionStatement(
            value: new UnaryOperationExpression(
                BasicType.Boolean,
                OperatorType.Not,
                Program.CreateExpression(type)
            )
        );

        Transpile();
    }

    // TODO: Add test for non-boolean value.
}