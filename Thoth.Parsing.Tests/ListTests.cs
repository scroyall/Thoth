using System.Data;
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
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.SymbolToken(SymbolType.LeftSquareBracket);
        Program.SymbolToken(SymbolType.RightSquareBracket);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());
        var definition = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.That(list.Values, Is.Empty);
    }

    [Test]
    public void List_WithOneExpression_HasOneMember()
    {
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.SymbolToken(SymbolType.LeftSquareBracket);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.RightSquareBracket);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());
        var definition = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.That(list.Values, Has.Count.EqualTo(1));
    }

    [Test]
    public void List_WithMultipleExpressions_HasMultipleMembers()
    {
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.SymbolToken(SymbolType.LeftSquareBracket);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Comma);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Comma);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.RightSquareBracket);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());
        var definition = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        Assert.That(definition.Value, Is.TypeOf<ListLiteralExpression>());
        var list = definition.Value as ListLiteralExpression ?? throw new NullReferenceException();

        Assert.That(list.Values, Has.Count.EqualTo(3));
    }
}
