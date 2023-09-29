using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class FunctionDefinitionTests
    : ParserTests
{
    [Test]
    public void FunctionDefinition_Parses_WithoutReturnType()
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

        Assert.That(program.Functions, Has.Count.EqualTo(1), "Expected exactly one function to be defined.");
        Assert.That(program.Functions.Keys, Has.Member(name), "Expected function name to be defined.");

        var function = program.Functions[name];

        Assert.That(function.Name, Is.EqualTo(name), "Expected defined function to have the correct name.");
    }

    [Test]
    public void FunctionDefinition_Parses_WithReturnType([Values] BasicType type)
    {
        var name = Fakes.IdentifierName;

        var program = Parse(
            Fakes.Keyword(KeywordType.Function),
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Minus),
            Fakes.Symbol(SymbolType.RightChevron),
            Fakes.Type(type),
            Fakes.Symbol(SymbolType.LeftBrace),
            Fakes.Symbol(SymbolType.RightBrace)
        );

        Assert.That(program.Functions, Has.Count.EqualTo(1), "Expected exactly one function to be defined.");
        Assert.That(program.Functions.Keys, Has.Member(name), "Expected function name to be defined.");

        var function = program.Functions[name];

        Assert.That(function.Name, Is.EqualTo(name), "Expected defined function to have the correct name.");
        Assert.That(function.ReturnType, Is.EqualTo(type), "Expected defined function to have the correct return type.");
    }
}