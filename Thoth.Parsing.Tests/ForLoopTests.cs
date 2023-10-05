using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class ForLoopTests
    : ParserTests
{
    [Test]
    public void ForLoop_WithRange_Parses()
    {
        Program.KeywordToken(KeywordType.For);
        Program.SymbolToken(SymbolType.LeftParenthesis);
        Program.IdentifierToken();
        Program.KeywordToken(KeywordType.In);
        Program.IntegerLiteralToken();
        Program.OperatorTokens(OperatorType.Range);
        Program.IntegerLiteralToken();
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<IteratorStatement>());
        var iterator = parsed.Statements[0] as IteratorStatement ?? throw new NullReferenceException();

        Assert.That(iterator.Iterable, Is.TypeOf<BinaryOperationExpression>());
        var binary = iterator.Iterable as BinaryOperationExpression ?? throw new NullReferenceException();

        Assert.That(binary.Operation, Is.EqualTo(OperatorType.Range));
    }

    [Test]
    public void ForLoop_WithListLiteral_Parses([Types] Type type)
    {
        Program.KeywordToken(KeywordType.For);
        Program.SymbolToken(SymbolType.LeftParenthesis);
        Program.IdentifierToken();
        Program.KeywordToken(KeywordType.In);
        Program.ListLiteralTokens(type);
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<IteratorStatement>());
        var iterator = parsed.Statements[0] as IteratorStatement ?? throw new NullReferenceException();

        Assert.That(iterator.Iterable, Is.TypeOf<ListLiteralExpression>()
                                             .With.Property("MemberType").EqualTo(type));
    }

    [Test]
    public void ForLoop_WithVariable_Parses()
    {
        Program.KeywordToken(KeywordType.For);
        Program.SymbolToken(SymbolType.LeftParenthesis);
        Program.IdentifierToken();
        Program.KeywordToken(KeywordType.In);
        var identifier = Program.IdentifierToken();
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<IteratorStatement>());
        var iterator = parsed.Statements[0] as IteratorStatement ?? throw new NullReferenceException();

        Assert.That(iterator.Iterable, Is.TypeOf<VariableExpression>()
                                             .With.Property("Identifier").EqualTo(identifier.Name));
    }
}
