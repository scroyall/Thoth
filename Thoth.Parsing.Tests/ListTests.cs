using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class ListTests
    : ParserTests
{
    [Test]
    public void List_WithNoMembers_Parses()
    {
        var program = Parse(
            Fakes.Keyword(KeywordType.Var),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.LeftSquareBracket),
            Fakes.Symbol(SymbolType.RightSquareBracket),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<VariableDefinitionStatement>());
        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.That(list.Values, Is.Empty);
    }

    [Test]
    public void List_WithOneMember_Parses()
    {
        var program = Parse(
            Fakes.Keyword(KeywordType.Var),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.LeftSquareBracket),
            Fakes.IntegerLiteral,
            Fakes.Symbol(SymbolType.RightSquareBracket),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<VariableDefinitionStatement>());
        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.That(list.Values, Has.Count.EqualTo(1));
    }

    [Test]
    public void List_WithMultipleMembers_Parses()
    {
        var program = Parse(
            Fakes.Keyword(KeywordType.Var),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.LeftSquareBracket),
            Fakes.IntegerLiteral,
            Fakes.Symbol(SymbolType.Comma),
            Fakes.IntegerLiteral,
            Fakes.Symbol(SymbolType.Comma),
            Fakes.IntegerLiteral,
            Fakes.Symbol(SymbolType.RightSquareBracket),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<VariableDefinitionStatement>());
        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.That(list.Values, Has.Count.EqualTo(3));
    }

    [Test]
    public void ListWithMultipleMembers_ThrowsException_WhenTypesMismatched()
    {
        Assert.Throws<MismatchedTypeException>(() =>
        {
            Parse(
                Fakes.Keyword(KeywordType.Var),
                Fakes.Identifier(),
                Fakes.Symbol(SymbolType.Equals),
                Fakes.Symbol(SymbolType.LeftSquareBracket),
                Fakes.IntegerLiteral,
                Fakes.Symbol(SymbolType.Comma),
                Fakes.BooleanLiteral,
                Fakes.Symbol(SymbolType.RightSquareBracket),
                Fakes.Symbol(SymbolType.Semicolon)
            );
        });
    }
}
