using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class ListTests
    : ParserTests
{
    [Test]
    public void List_WithNoExpressions_HasNoMembers()
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
    public void ListTypeParameter_IsResolved_WhenMembersTypesAreResolved(
        [ResolvedTypes] IResolvedType type)
    {
        var tokens = new List<Token> {
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Symbol(SymbolType.LeftSquareBracket)
        };
        Fakes.Literal(ref tokens, type);
        tokens.Add(Fakes.Symbol(SymbolType.Comma));
        Fakes.Literal(ref tokens, type);
        tokens.Add(Fakes.Symbol(SymbolType.Comma));
        Fakes.Literal(ref tokens, type);
        tokens.Add(Fakes.Symbol(SymbolType.RightSquareBracket));
        tokens.Add(Fakes.Symbol(SymbolType.Semicolon));

        var program = Parse(tokens);

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<VariableDefinitionStatement>());
        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.That(list.Type, Is.TypeOf<ParameterizedType>());
        var parameterized = list.Type as ParameterizedType ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(parameterized.Outer, Is.EqualTo(BasicType.List));
            Assert.That(parameterized.Parameter, Is.EqualTo(type.Resolve()));
        });
    }

    [Test]
    public void ListTypeParameter_IsUnresolved_WhenMembersTypesAreUnresolved()
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

        Assert.That(list.Type, Is.TypeOf<ParameterizedType>());
        var parameterized = list.Type as ParameterizedType ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(parameterized.Outer, Is.EqualTo(BasicType.List));
            Assert.That(parameterized.Parameter, Is.EqualTo(BasicType.Unresolved));
        });
    }

    [Test]
    public void ListTypeParameter_IsUnresolved_WhenUnparameterized([ResolvedTypes] IResolvedType type)
    {
        var program = Parse(
            Fakes.Type(BasicType.List),
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

        Assert.That(list.Type, Is.TypeOf<ParameterizedType>());
        var parameterized = list.Type as ParameterizedType ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(parameterized.Outer, Is.EqualTo(BasicType.List));
            Assert.That(parameterized.Parameter, Is.EqualTo(BasicType.Unresolved));
        });
    }

    [Test]
    public void ListTypeParameter_TypeIsResolved_WhenParameterized([ResolvedTypes] IResolvedType typeParameter)
    {
        var program = Parse(
            Fakes.Type(BasicType.List),
            Fakes.Symbol(SymbolType.LeftChevron),
            Fakes.Type(typeParameter),
            Fakes.Symbol(SymbolType.RightChevron),
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

        Assert.That(definition.Type, Is.TypeOf<ParameterizedType>());
        var type = definition.Type as ParameterizedType ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(type.Outer, Is.EqualTo(BasicType.List));
            Assert.That(type.Parameter, Is.EqualTo(typeParameter));
        });
    }
}
