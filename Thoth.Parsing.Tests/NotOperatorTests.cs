using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class NotOperatorTests
    : ParserTests
{
    [Test]
    public void NotOperator_Parses_WhenOperandIsBoolean()
    {
        var program = Parse(
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.Exclamation),
            Fakes.BooleanLiteral,
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assume.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assume.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(BasicType.Boolean),
                                           "Expected definition statement of boolean type.");

        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();
        Assert.That(definition.Value, Is.TypeOf<UnaryOperationExpression>()
                                        .With.Property("Operation").EqualTo(OperatorType.Not),
                                        "Expected unary expression for not operation.");
    }

    [Test]
    public void NotOperator_Parses_WhenOperandTypeIsUnresolved()
    {
        var program = Parse(
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.Exclamation),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assume.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assume.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(BasicType.Boolean),
                                           "Expected definition statement of boolean type.");

        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();
        Assert.That(definition.Value, Is.TypeOf<UnaryOperationExpression>()
                                        .With.Property("Operation").EqualTo(OperatorType.Not),
                                        "Expected unary expression for not operation.");
    }

    [Test]
    public void NotOperator_ThrowsException_WhenOperandIsNonBooleanLiteral(
        [ValueSource(typeof(ResolvedBasicType), nameof(ResolvedBasicType.Values))] ResolvedBasicType type)
    {
        if (type.Matches(BasicType.Boolean)) Assert.Ignore("Value type is boolean.");

        Assert.Throws<MismatchedTypeException>(() => Parse(
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.Exclamation),
            Fakes.Literal(type),
            Fakes.Symbol(SymbolType.Semicolon)
        ));
    }
}