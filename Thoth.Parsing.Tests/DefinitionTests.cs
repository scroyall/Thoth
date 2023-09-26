using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class DefinitionTests
    : ParserTests
{
    [Test]
    public void DefinitionStatement_Parses_WhenVariableTypeIsUnresolved_AndExpressionIsLiteral([Values] BasicType type)
    {
        if (type == BasicType.Boolean) Assert.Ignore("Boolean literals are not implemented.");
        if (type == BasicType.String)  Assert.Ignore("String literal assignment is not implemented.");

        var program = Parse(
            Fakes.Keyword(KeywordType.Var),
            Fakes.Identifier("value"),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Literal(type),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement of resolved type {type}.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .With.Property("Value")
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement with value of resolved type {type}.");
    }

    [Test]
    public void DefinitionStatement_Parses_WhenVariableTypeIsUnresolved_AndExpressionTypeIsUnresolved()
    {
        var program = Parse(
            Fakes.Keyword(KeywordType.Var),
            Fakes.Identifier("value"),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .With.Property("Type").EqualTo(null),
                                           $"Expected definition statement of unresolved type.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .With.Property("Value")
                                           .With.Property("Type").EqualTo(null),
                                           $"Expected definition statement with value of unresolved type.");
    }

    [Test]
    public void DefinitionStatement_Parses_WhenVariableTypeIsResolved_AndExpressionIsMatchingLiteral([Values] BasicType type)
    {

        if (type == BasicType.Boolean) Assert.Ignore("Boolean literals are not implemented.");
        if (type == BasicType.String)  Assert.Ignore("String literal assignment is not implemented.");

        var program = Parse(
            Fakes.Type(type),
            Fakes.Identifier("value"),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Literal(type),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement with resolved type {type}.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .With.Property("Value")
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement with value of resolved type {type}.");
    }

    [Test]
    public void DefinitionStatement_Parses_WhenVariableTypeIsResolved_AndExpressionTypeIsUnresolved([Values] BasicType type)
    {
        var program = Parse(
            Fakes.Type(type),
            Fakes.Identifier("value"),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement with resolved type {type}.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
                                           .With.Property("Value")
                                           .With.Property("Type").EqualTo(null),
                                           $"Expected definition statement with value of unresolved type.");
    }
}