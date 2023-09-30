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

    public static TokenizedProgram Program(IEnumerable<Token> tokens)
        => new(tokens.ToList(), new List<string>{ "FakeString" });

    public static BooleanLiteralToken BooleanLiteral
        => new(true, SourceReference);
    
    public static IdentifierToken Identifier(string? name = null)
        => new(name ?? IdentifierName, SourceReference);

    private static int IdentifierCount = 0;

    public static string IdentifierName
        => $"identifier{++IdentifierCount}";

    public static IntegerLiteralToken IntegerLiteral
        => new(0, SourceReference);

    public static KeywordToken Keyword(KeywordType type)
        => new(type, SourceReference);

    public static Token Literal(BasicType type)
    {
        return type switch
        {
            BasicType.Integer => IntegerLiteral,
            BasicType.String  => StringLiteral,
            BasicType.Boolean => BooleanLiteral,
            _ => throw new NotImplementedException()
        };
    }

    public static NamedParameter NamedParameter(BasicType type)
        => new(type, IdentifierName);

    public static void Operation(ref List<Token> tokens, OperatorType operation)
    {
        switch (operation)
        {
            case OperatorType.And:
                tokens.Add(Symbol(SymbolType.Ampersand));
                tokens.Add(Symbol(SymbolType.Ampersand));
                break;
            case OperatorType.Or:
                tokens.Add(Symbol(SymbolType.VerticalBar));
                tokens.Add(Symbol(SymbolType.VerticalBar));
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static StringLiteralToken StringLiteral
        => new(0, SourceReference);

    public static SymbolToken Symbol(SymbolType type)
        => new(type, SourceReference);

    public static TypeToken Type(BasicType type)
        => new(type, SourceReference);
}