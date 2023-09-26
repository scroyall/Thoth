using NUnit.Framework.Constraints;
using Thoth.Tokenization;
using Thoth.Tokenization.Tokens;

namespace Thoth.Parsing.Tests;

public static class Fakes
{
    public static SourceReference SourceReference
        => new(-1, -1);

    public static TokenizedProgram Program(string source)
        => new Tokenizer().Tokenize(source);

    public static TokenizedProgram Program(params Token[] tokens)
        => new(tokens, new List<string>{ "FakeString" });
    
    public static IdentifierToken Identifier(string name = "fake")
        => new(name, SourceReference);

    public static IntegerLiteralToken IntegerLiteral
        => new(0, SourceReference);

    public static KeywordToken Keyword(KeywordType type)
        => new(type, SourceReference);

    public static Token Literal(BasicType type)
    {
        return type switch
        {
            BasicType.Integer => IntegerLiteral,
            BasicType.String => StringLiteral,
            _ => throw new NotImplementedException()
        };
    }

    public static StringLiteralToken StringLiteral
        => new(0, SourceReference);

    public static SymbolToken Symbol(SymbolType type)
        => new(type, SourceReference);

    public static TypeToken Type(BasicType type)
        => new(type, SourceReference);
}