using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class VariableDefinitionTests
    : ParserTests
{
    [Test]
    public void VariableDefinitionStatement_WithLiteralValue_Parses([Types] Type type)
    {
        Program.TypeTokens(type);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.LiteralTokens(type);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(parsed.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement of type {type}.");
    }

    [Test]
    public void VariableDefinitionStatement_WithVariableValue_Parses([Types] Type type)
    {
        Program.TypeTokens(type);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(parsed.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(type),
                                           $"Expected definition statement of type {type}.");
    }
}
