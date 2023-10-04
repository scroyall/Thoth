using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class ListLiteralTests
    : ParserTests
{
    [Test]
    public void ListLiteral_WithNoExpressions_HasNoMembers([Types] Type memberType)
    {
        // l = list<MEMBER_TYPE>[];
        Program.IdentifierToken("l");
        Program.SymbolToken(SymbolType.Equals);
        Program.BuiltinTypeToken(BuiltinType.List);
        Program.SymbolToken(SymbolType.LeftChevron);
        Program.TypeTokens(memberType);
        Program.SymbolToken(SymbolType.RightChevron);
        Program.SymbolToken(SymbolType.LeftSquareBracket);
        Program.SymbolToken(SymbolType.RightSquareBracket);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());
        var definition = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(list.Type, Is.EqualTo(Type.List(memberType)));
            Assert.That(list.Values, Is.Empty);
        });
    }

    [Test]
    public void ListLiteral_WithOneExpression_HasOneMember([Types] Type memberType)
    {
        // l = list<MEMBER_TYPE>[x];
        Program.IdentifierToken("l");
        Program.SymbolToken(SymbolType.Equals);
        Program.BuiltinTypeToken(BuiltinType.List);
        Program.SymbolToken(SymbolType.LeftChevron);
        Program.TypeTokens(memberType);
        Program.SymbolToken(SymbolType.RightChevron);
        Program.SymbolToken(SymbolType.LeftSquareBracket);
        Program.IdentifierToken("x");
        Program.SymbolToken(SymbolType.RightSquareBracket);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());
        var definition = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(list.Type, Is.EqualTo(Type.List(memberType)));
            Assert.That(list.Values, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void ListLiteral_WithMultipleExpressions_HasMultipleMembers([Types] Type memberType)
    {
        // l = list<MEMBER_TYPE>[x, y, z];
        Program.IdentifierToken("l");
        Program.SymbolToken(SymbolType.Equals);
        Program.BuiltinTypeToken(BuiltinType.List);
        Program.SymbolToken(SymbolType.LeftChevron);
        Program.TypeTokens(memberType);
        Program.SymbolToken(SymbolType.RightChevron);
        Program.SymbolToken(SymbolType.LeftSquareBracket);
        Program.IdentifierToken("x");
        Program.SymbolToken(SymbolType.Comma);
        Program.IdentifierToken("y");
        Program.SymbolToken(SymbolType.Comma);
        Program.IdentifierToken("z");
        Program.SymbolToken(SymbolType.RightSquareBracket);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());
        var definition = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(list.Type, Is.EqualTo(Type.List(memberType)));
            Assert.That(list.Values, Has.Count.EqualTo(3));
        });
    }
}
