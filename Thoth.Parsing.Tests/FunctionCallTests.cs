using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class FunctionCallTests
    : ParserTests
{
    [Test]
    public void FunctionCall_Parses_AsStatement()
    {
        var name = Fakes.IdentifierName;

        var program = Parse(
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");

        var call = program.Statements[0] as FunctionCallStatement ?? throw new NullReferenceException();

        Assert.That(call.Name, Is.EqualTo(name), "Expected function call name to match identifier.");
    }

    [Test]
    public void FunctionCall_Parses_AsExpression()
    {
        var name = Fakes.IdentifierName;

        var program = Parse(
            Fakes.Keyword(KeywordType.Print),
            Fakes.Symbol(SymbolType.LeftParenthesis),

            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Symbol(SymbolType.RightParenthesis),

            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).InstanceOf<PrintStatement>(), "Expected print statement.");

        var print = program.Statements[0] as PrintStatement ?? throw new NullReferenceException();

        Assert.That(print.Value, Is.InstanceOf<FunctionCallExpression>(), "Expected function call expression.");

        var call = print.Value as FunctionCallExpression ?? throw new NullReferenceException();

        Assert.That(call.Name, Is.EqualTo(name), "Expected function call name to match identifier.");
    }
}