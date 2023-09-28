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
            Fakes.Keyword(KeywordType.Var),
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
            Fakes.Keyword(KeywordType.Var),
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
    public void NotOperator_Parses_WhenOperandIsNotBoolean([Values] BasicType valueType)
    {
        if (valueType == BasicType.Boolean) Assert.Ignore("Value type is boolean.");
        if (valueType == BasicType.String)  Assert.Ignore("String literals cannot be parsed as expressions yet.");

        Assert.Throws<MismatchedTypeException>(() => Parse(
            Fakes.Keyword(KeywordType.Var),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.Exclamation),
            Fakes.Literal(valueType),
            Fakes.Symbol(SymbolType.Semicolon)
        ));
    }
}