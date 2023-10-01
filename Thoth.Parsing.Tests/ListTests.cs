using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class ListTests
    : ParserTests
{
    [Test]
    public void List_WithNoExpression_HasNoMembers()
    {
        var program = Parse(
            Fakes.Type(BasicType.Unresolved),
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
    public void List_WithOneExpression_HasOneMember()
    {
        var program = Parse(
            Fakes.Type(BasicType.Unresolved),
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
    public void List_WithMultipleExpressions_HasMultipleMembers()
    {
        var program = Parse(
            Fakes.Type(BasicType.Unresolved),
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
    public void List_ThrowsException_WhenMembersTypesDoNotMatch()
    {
        Assert.Throws<MismatchedTypeException>(() =>
        {
            Parse(
                Fakes.Type(BasicType.Unresolved),
                Fakes.Identifier(),
                Fakes.Symbol(SymbolType.Equals),
                Fakes.Symbol(SymbolType.LeftSquareBracket),
                Fakes.IntegerLiteral,
                Fakes.Symbol(SymbolType.Comma),
                Fakes.BooleanLiteral,
                Fakes.Symbol(SymbolType.Comma),
                Fakes.StringLiteral,
                Fakes.Symbol(SymbolType.RightSquareBracket),
                Fakes.Symbol(SymbolType.Semicolon)
            );
        });
    }

    [Test]
    public void List_AssumesMembersType_WhenMembersTypesAreResolved(
        [ValueSource(typeof(ResolvedBasicType), nameof(ResolvedBasicType.Values))] ResolvedBasicType type)
    {
        var program = Parse(
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.LeftSquareBracket),
            Fakes.Literal(type),
            Fakes.Symbol(SymbolType.Comma),
            Fakes.Literal(type),
            Fakes.Symbol(SymbolType.Comma),
            Fakes.Literal(type),
            Fakes.Symbol(SymbolType.RightSquareBracket),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<VariableDefinitionStatement>());
        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.That(list.Type, Is.EqualTo(type));
    }

    [Test]
    public void List_TypeIsUnresolved_WhenMembersTypesAreUnresolved()
    {
        var program = Parse(
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.LeftSquareBracket),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Comma),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Comma),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.RightSquareBracket),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<VariableDefinitionStatement>());
        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.That(list.Type, Is.EqualTo(BasicType.Unresolved));
    }
}
