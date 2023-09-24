using Thoth.Parsing;

namespace Thoth.Transpilation.Tests;

public class AssignmentStatementTests
    : TranspilerTests
{
    [Test]
    public void Assignment_DoesNotThrow_WhenExpressionTypeMatchesDefinition(
        [ValueSource(nameof(Types))] BasicType type)
    {
        var definition = Fakes.Definition(type);

        Transpile(
            definition,
            Fakes.Assignment(definition.Identifier, type)
        );
    }

    [Test]
    public void Assignment_ThrowsUnresolvedTypeException_WhenExpressionTypeIsUnresolved(
        [ValueSource(nameof(Types))] BasicType variableType)
    {
        var definition = Fakes.Definition(variableType);

        Assert.Throws<UnresolvedTypeException>(() => Transpile(
            definition,
            Fakes.Assignment(definition.Identifier, Fakes.UnresolvedType)
        ));
    }

    [Test]
    public void Assignment_ThrowsMismatchedTypeException_WhenExpressionTypeDiffersFromDefinition(
        [ValueSource(nameof(Types))] BasicType variableType,
        [ValueSource(nameof(Types))] BasicType expressionType)
    {
        if (expressionType == variableType) Assert.Ignore("Expression type matches variable type.");

        var definition = Fakes.Definition(variableType);

        Assert.Throws<MismatchedTypeException>(() => Transpile(
            definition,
            Fakes.Assignment(definition.Identifier, expressionType)
        ));
    }
}