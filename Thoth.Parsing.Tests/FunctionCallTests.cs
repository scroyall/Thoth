using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class FunctionCallTests
    : ParserTests
{
    [Test]
    public void FunctionCall_Parses()
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
}