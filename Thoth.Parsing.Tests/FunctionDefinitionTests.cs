using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class FunctionDefinitionTests
    : ParserTests
{
    [Test]
    public void FunctionDefinitionStatement_AddsDefinedFunction_WhenParsed()
    {
        var name = Fakes.IdentifierName;

        var program = Parse(
            Fakes.Keyword(KeywordType.Function),
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.LeftBrace),
            Fakes.Symbol(SymbolType.RightBrace)
        );

        Assert.That(program.Functions, Has.Count.EqualTo(1), "Expected exactly one function.");
        Assert.That(program.Functions.Keys, Has.Member(name), "Expected defined function for name.");

        var function = program.Functions[name];

        Assert.That(function.Name, Is.EqualTo(name), "Expected defined function to have matching name.");
    }
}