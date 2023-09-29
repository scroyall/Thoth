using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class ReturnTests
    : ParserTests
{
    [Test]
    public void Return_Parses_WithoutValue()
    {
        var program = Parse(
            Fakes.Keyword(KeywordType.Return),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<ReturnStatement>(), "Expected return statement.");

        var statement = program.Statements[0] as ReturnStatement ?? throw new NullReferenceException();

        Assert.That(statement.Value, Is.Null, "Expected no return value.");
    }

    [Test]
    public void Return_Parses_WithLiteralValue([Values] BasicType type)
    {
        var program = Parse(
            Fakes.Keyword(KeywordType.Return),
            Fakes.Literal(type),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<ReturnStatement>(), "Expected return statement.");

        var statement = program.Statements[0] as ReturnStatement ?? throw new NullReferenceException();

        Assert.That(statement.Value, Is.Not.Null, "Expected return value.");
    }
}