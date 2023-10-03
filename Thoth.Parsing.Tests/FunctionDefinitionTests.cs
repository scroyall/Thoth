using Thoth.Parsing.Statements;
using Thoth.Tests;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class FunctionDefinitionTests
    : ParserTests
{
    [Test]
    public void FunctionDefinition_WithoutReturnType_Parses()
    {
        Program.KeywordToken(KeywordType.Function);
        var functionName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        var parsed = Parse();

        Assert.That(parsed.Functions, Contains.Key(functionName).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = parsed.Functions[functionName] ?? throw new NullReferenceException();
            Assert.That(function.Name, Is.EqualTo(functionName));
            Assert.That(function.Parameters, Is.Empty);
            Assert.That(function.ReturnType, Is.Null);
            Assert.That(function.Body, Is.TypeOf<ScopeStatement>().With.Property("Statements").Empty);
        });
    }

    [Test]
    public void FunctionDefinition_WithStatement_Parses()
    {
        Program.KeywordToken(KeywordType.Function);
        var functionName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.KeywordToken(KeywordType.Return);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Functions, Contains.Key(functionName).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = parsed.Functions[functionName];
            Assert.That(function.Name, Is.EqualTo(functionName));
            Assert.That(function.Parameters, Is.Empty);
            Assert.That(function.ReturnType, Is.Null);
            Assert.That(function.Body, Is.TypeOf<ReturnStatement>());
        });
    }

    [Test]
    public void FunctionDefinition_WithMissingReturnType_ThrowsException()
    {
        Program.KeywordToken(KeywordType.Function);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.LeftParenthesis);
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.Minus);
        Program.SymbolToken(SymbolType.RightChevron);
        // Missing return type.
        Program.KeywordToken(KeywordType.Return);
        Program.SymbolToken(SymbolType.Semicolon);

        Assert.Throws<UnexpectedTokenException>(() => Parse());
    }

    [Test]
    public void FunctionDefinition_WithReturnType_Parses([Types] Type type)
    {
        Program.KeywordToken(KeywordType.Function);
        var functionName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.Minus);
        Program.SymbolToken(SymbolType.RightChevron);
        Program.TypeTokens(type);
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        var parsed = Parse();

        Assert.That(parsed.Functions, Contains.Key(functionName).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = parsed.Functions[functionName];
            Assert.That(function.Name, Is.EqualTo(functionName));
            Assert.That(function.Parameters, Is.Empty);
            Assert.That(function.ReturnType, Is.EqualTo(type));
            Assert.That(function.Body, Is.TypeOf<ScopeStatement>().With.Property("Statements").Empty);
        });
    }

    [Test]
    public void FunctionDefinition_WithSingleParameter_Parses([Types] Type type)
    {
        Program.KeywordToken(KeywordType.Function);
        var functionName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        var parameter = Program.NamedParameterTokens(type: type);
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        var parsed = Parse();

        Assert.That(parsed.Functions, Contains.Key(functionName).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = parsed.Functions[functionName];
            Assert.That(function.Name, Is.EqualTo(functionName));
            Assert.That(function.Parameters, Is.EquivalentTo(new[] { parameter }));
            Assert.That(function.ReturnType, Is.Null);
            Assert.That(function.Body, Is.TypeOf<ScopeStatement>().With.Property("Statements").Empty);
        });
    }

    [Test]
    public void FunctionDefinition_WithMultipleParameters_Parses([Types] Type firstType, [Types] Type secondType)
    {
        Program.KeywordToken(KeywordType.Function);
        var functionName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        var firstParameter = Program.NamedParameterTokens(type: firstType);
        Program.SymbolToken(SymbolType.Comma);
        var secondParameter = Program.NamedParameterTokens(type: secondType);
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        var parsed = Parse();

        Assert.That(parsed.Functions, Contains.Key(functionName).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = parsed.Functions[functionName];
            Assert.That(function.Name, Is.EqualTo(functionName));
            Assert.That(parsed.Functions[functionName].Parameters, Is.EquivalentTo(new[] { firstParameter, secondParameter }));
            Assert.That(function.ReturnType, Is.Null);
            Assert.That(function.Body, Is.TypeOf<ScopeStatement>().With.Property("Statements").Empty);
        });
    }
}
