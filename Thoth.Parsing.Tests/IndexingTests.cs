using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;
using Thoth.Tokenization.Tokens;

namespace Thoth.Parsing.Tests;

public class IndexingTests
    : ParserTests
{
    [Test]
    public void ListIndexOperator_WithoutIndex_ThrowsException()
    {
        // m = l[];
        Program.IdentifierToken("m");
        Program.SymbolToken(SymbolType.Equals);
        Program.IdentifierToken("l");
        Program.SymbolToken(SymbolType.LeftSquareBracket);
        Program.SymbolToken(SymbolType.RightSquareBracket);
        Program.SymbolToken(SymbolType.Semicolon);

        Assert.Throws<UnexpectedTokenException>(() => Parse());
    }

    [Test]
    public void ListIndexOperator_WithIntegerLiteralIndex_Parses()
    {
        // m = l[0];
        Program.IdentifierToken("m");
        Program.SymbolToken(SymbolType.Equals);
        Program.IdentifierToken("l");
        Program.SymbolToken(SymbolType.LeftSquareBracket);
        Program.IntegerLiteralToken();
        Program.SymbolToken(SymbolType.RightSquareBracket);
        Program.SymbolToken(SymbolType.Semicolon);

        var value = ParseAssignmentValue();

        Assert.That(value, Is.TypeOf<IndexExpression>());
        var index = value as IndexExpression ?? throw new NullReferenceException();

        Assert.That(index.Index, Is.TypeOf<IntegerExpression>());
    }
}
