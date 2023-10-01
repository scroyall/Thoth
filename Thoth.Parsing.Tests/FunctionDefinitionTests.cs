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
        var name = Fakes.IdentifierName;
        var program = Parse(
            Fakes.Keyword(KeywordType.Function),
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.LeftBrace),
            Fakes.Symbol(SymbolType.RightBrace)
        );

        Assert.That(program.Functions, Contains.Key(name).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = program.Functions[name] ?? throw new NullReferenceException();
            Assert.That(function.Name, Is.EqualTo(name));
            Assert.That(function.Parameters, Is.Empty);
            Assert.That(function.ReturnType, Is.Null);
            Assert.That(function.Body, Is.TypeOf<ScopeStatement>().With.Property("Statements").Empty);
        });
    }

    [Test]
    public void FunctionDefinition_WithStatement_Parses()
    {
        var name = Fakes.IdentifierName;
        var program = Parse(
            Fakes.Keyword(KeywordType.Function),
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Keyword(KeywordType.Return),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Functions, Contains.Key(name).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = program.Functions[name];
            Assert.That(function.Name, Is.EqualTo(name));
            Assert.That(function.Parameters, Is.Empty);
            Assert.That(function.ReturnType, Is.Null);
            Assert.That(function.Body, Is.TypeOf<ReturnStatement>());
        });
    }

    [Test]
    public void FunctionDefinition_WithMissingReturnType_ThrowsException()
    {
        Assert.Throws<UnexpectedTokenException>(() =>
        {
            Parse(
                Fakes.Keyword(KeywordType.Function),
                Fakes.Identifier(),
                Fakes.Symbol(SymbolType.LeftParenthesis),
                Fakes.Symbol(SymbolType.RightParenthesis),
                Fakes.Symbol(SymbolType.Minus),
                Fakes.Symbol(SymbolType.RightChevron),
                // Missing return type.
                Fakes.Symbol(SymbolType.LeftBrace),
                Fakes.Symbol(SymbolType.RightBrace)
            );
        });
    }

    [Test]
    public void FunctionDefinition_WithResolvedReturnType_Parses(
        [ResolvedTypes()] IResolvedType type)
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

        Assert.That(program.Functions, Contains.Key(name).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = program.Functions[name];
            Assert.That(function.Name, Is.EqualTo(name));
            Assert.That(function.Parameters, Is.Empty);
            Assert.That(function.ReturnType, Is.EqualTo(type));
            Assert.That(function.Body, Is.TypeOf<ScopeStatement>().With.Property("Statements").Empty);
        });
    }

    [Test]
    public void FunctionDefinition_WithUnresolvedReturnType_ThrowsException()
    {
        Assert.Throws<UnresolvedTypeException>(() => Parse(
            Fakes.Keyword(KeywordType.Function),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Minus),
            Fakes.Symbol(SymbolType.RightChevron),
            Fakes.Type(BasicType.Unresolved),
            Fakes.Symbol(SymbolType.LeftBrace),
            Fakes.Symbol(SymbolType.RightBrace)
        ));
    }

    [Test]
    public void FunctionDefinition_WithSingleResolvedParameter_Parses(
        [ValueSource(typeof(ResolvedBasicType), nameof(ResolvedBasicType.Values))] IResolvedType type)
    {
        var parameter = Fakes.NamedParameter(type);

        var name = Fakes.IdentifierName;
        var program = Parse(
            Fakes.Keyword(KeywordType.Function),
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Type(parameter.Type),
            Fakes.Identifier(parameter.Name),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.LeftBrace),
            Fakes.Symbol(SymbolType.RightBrace)
        );

        Assert.That(program.Functions, Contains.Key(name).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = program.Functions[name];
            Assert.That(function.Name, Is.EqualTo(name));
            Assert.That(function.Parameters, Is.EquivalentTo(new[] { parameter }));
            Assert.That(function.ReturnType, Is.Null);
            Assert.That(function.Body, Is.TypeOf<ScopeStatement>().With.Property("Statements").Empty);
        });
    }

    [Test]
    public void FunctionDefinition_WithSingleUnresolvedParameter_ThrowsException()
    {
        Assert.Throws<UnresolvedTypeException>(() => Parse(
            Fakes.Keyword(KeywordType.Function),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.LeftBrace),
            Fakes.Symbol(SymbolType.RightBrace)
        ));
    }

    [Test]
    public void FunctionDefinition_ParsesWithMultipleResolvedParameters(
        [ValueSource(typeof(ResolvedBasicType), nameof(ResolvedBasicType.Values))] IResolvedType firstType,
        [ValueSource(typeof(ResolvedBasicType), nameof(ResolvedBasicType.Values))] IResolvedType secondType)
    {
        var first = Fakes.NamedParameter(firstType);
        var second = Fakes.NamedParameter(secondType);

        var name = Fakes.IdentifierName;
        var program = Parse(
            Fakes.Keyword(KeywordType.Function),
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Type(first.Type),
            Fakes.Identifier(first.Name),
            Fakes.Symbol(SymbolType.Comma),
            Fakes.Type(second.Type),
            Fakes.Identifier(second.Name),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.LeftBrace),
            Fakes.Symbol(SymbolType.RightBrace)
        );

        Assert.That(program.Functions, Contains.Key(name).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var function = program.Functions[name];
            Assert.That(function.Name, Is.EqualTo(name));
            Assert.That(program.Functions[name].Parameters, Is.EquivalentTo(new[] { first, second }));
            Assert.That(function.ReturnType, Is.Null);
            Assert.That(function.Body, Is.TypeOf<ScopeStatement>().With.Property("Statements").Empty);
        });
    }
}
