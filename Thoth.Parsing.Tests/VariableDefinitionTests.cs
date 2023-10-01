using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class VariableDefinitionTests
    : ParserTests
{
    [Test]
    public void VariableDefinitionStatement_Parses_WhenVariableTypeIsUnresolved_AndExpressionIsLiteral(
        [ResolvedTypes] IResolvedType type)
    {
        var tokens = new List<Token> {
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier("value"),
            Fakes.Symbol(SymbolType.Equals),
        };
        Fakes.Literal(ref tokens, type);
        tokens.Add(Fakes.Symbol(SymbolType.Semicolon));

        var program = Parse(tokens);

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement of resolved type {type}.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Value")
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement with value of resolved type {type}.");
    }

    [Test]
    public void VariableDefinitionStatement_Parses_WhenVariableTypeIsUnresolved_AndExpressionTypeIsUnresolved()
    {
        var program = Parse(
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier("value"),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(BasicType.Unresolved),
                                           $"Expected definition statement of unresolved type.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Value")
                                           .With.Property("Type").EqualTo(BasicType.Unresolved),
                                           $"Expected definition statement with value of unresolved type.");
    }

    [Test]
    public void VariableDefinitionStatement_Parses_WhenVariableTypeIsResolved_AndExpressionIsMatchingLiteral(
        [ResolvedTypes] IResolvedType type)
    {
        var tokens = new List<Token> {
            Fakes.Type(type),
            Fakes.Identifier("value"),
            Fakes.Symbol(SymbolType.Equals)
        };
        Fakes.Literal(ref tokens, type);
        tokens.Add(Fakes.Symbol(SymbolType.Semicolon));

        var program = Parse(tokens);

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement with resolved type {type}.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Value")
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement with value of resolved type {type}.");
    }

    [Test]
    public void VariableDefinitionStatement_Parses_WhenVariableTypeIsResolved_AndExpressionTypeIsUnresolved(
        [ResolvedTypes] IResolvedType type)
    {
        var program = Parse(
            Fakes.Type(type),
            Fakes.Identifier("value"),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement with resolved type {type}.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Value")
                                           .With.Property("Type").EqualTo(BasicType.Unresolved),
                                           $"Expected definition statement with value of unresolved type.");
    }
}