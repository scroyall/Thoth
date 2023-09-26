using Thoth.Parsing;

namespace Thoth.Transpilation.Tests;

public class DefinitionStatementTests
    : TranspilerTests
{
    [Test]
    public void DefinitionStatement_Transpiles_WhenExpressionTypeEqualsVariableType([Values] BasicType type)
    {
        Transpile(
            Fakes.Definition(type)
        );
    }

    [Test]
    public void DefinitionStatement_Transpiles_WhenVariableTypeIsUnresolved([Values] BasicType expressionType)
    {
        Transpile(
            Fakes.Definition(null, expression: Fakes.Expression(expressionType))
        );
    }

    [Test]
    public void DefinitionStatement_ThrowsMismatchedTypeException_WhenExpressionTypeDoesNotMatchVariableType(
        [Values] BasicType variableType,
        [Values] BasicType expressionType)
    {
        if (expressionType.Matches(variableType)) Assert.Ignore("Expression type matches variable type.");

        Assert.Throws<MismatchedTypeException>(() => Transpile(
            Fakes.Definition(variableType, expression: Fakes.Expression(expressionType))
        ));
    }
}