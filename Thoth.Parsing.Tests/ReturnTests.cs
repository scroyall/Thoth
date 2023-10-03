using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class ReturnTests
    : ParserTests
{
    [Test]
    public void Return_Parses_WithoutValue()
    {
        Program.KeywordToken(KeywordType.Return);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(parsed.Statements, Has.Exactly(1).TypeOf<ReturnStatement>(), "Expected return statement.");

        var statement = parsed.Statements[0] as ReturnStatement ?? throw new NullReferenceException();

        Assert.That(statement.Value, Is.Null, "Expected no return value.");
    }

    [Test]
    public void Return_Parses_WithLiteralValue([Types] Type type)
    {
        Program.KeywordToken(KeywordType.Return);
        Program.LiteralTokens(type);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(parsed.Statements, Has.Exactly(1).TypeOf<ReturnStatement>(), "Expected return statement.");

        var statement = parsed.Statements[0] as ReturnStatement ?? throw new NullReferenceException();

        Assert.That(statement.Value, Is.Not.Null, "Expected return value.");
    }
}