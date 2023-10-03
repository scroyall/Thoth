using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using Thoth.Tokenization;
using Thoth.Tokenization.Tokens;

namespace Thoth.Parsing.Tests;

public class TokenizedProgramFaker
{
    private int _names = 0;

    public List<Token> Tokens = new();

    public List<string> Strings = new();

    public static SourceReference CreateSourceReference()
        => new(-1, -1);

    public TToken AddToken<TToken>(TToken token)
        where TToken : Token
    {
        Tokens.Add(token);
        return token;
    }

    // Types
    //

    public BuiltinTypeToken BuiltinTypeToken(BuiltinType type)
        => AddToken(new BuiltinTypeToken(type, CreateSourceReference()));
    
    public void TypeTokens(Type type)
    {
        BuiltinTypeToken(type.Root);

        if (type.HasParameters)
        {
            SymbolToken(SymbolType.LeftChevron);

            TypeTokens(type.Parameters[0]);

            foreach (var parameter in type.Parameters.Skip(1))
            {
                SymbolToken(SymbolType.Comma);
                TypeTokens(parameter);
            }

            SymbolToken(SymbolType.RightChevron);
        }
    }

    // Identifiers
    //

    public IdentifierToken IdentifierToken(string? name = null)
        => AddToken(CreateIdentifierToken(name));

    public IdentifierToken CreateIdentifierToken(string? name = null)
        => new(name ?? CreateIdentifierName(), CreateSourceReference());

    public string CreateIdentifierName()
        => $"identifier{++_names}";

    // Symbols
    //

    public void SymbolToken(SymbolType type)
        => Tokens.Add(new SymbolToken(type, CreateSourceReference()));

    public TokenizedProgram ToTokenizedProgram()
        => new(Tokens, Strings);
    
    // Literals
    //

    public void LiteralTokens(Type type)
    {
        switch (type.Root)
        {
            case BuiltinType.Int:
                Tokens.Add(new IntegerLiteralToken(0, CreateSourceReference()));
                break;
            case BuiltinType.Bool:
                Tokens.Add(new BooleanLiteralToken(false, CreateSourceReference()));
                break;
            case BuiltinType.String:
                Tokens.Add(new StringLiteralToken(0, CreateSourceReference()));
                break;
            case BuiltinType.List:
                ListLiteralTokens(type.Parameters[0]);
                break;
        }
    }

    public void ListLiteralTokens(Type type)
    {
        SymbolToken(SymbolType.LeftSquareBracket);

        LiteralTokens(type);
        SymbolToken(SymbolType.Comma);
        LiteralTokens(type);
        SymbolToken(SymbolType.Comma);
        LiteralTokens(type);

        SymbolToken(SymbolType.RightSquareBracket);
    }

    // Operators
    //

    public void OperatorTokens(OperatorType type)
    {
        switch (type)
        {
            case OperatorType.Add:
                SymbolToken(SymbolType.Plus);
                break;
            case OperatorType.Subtract:
                SymbolToken(SymbolType.Minus);
                break;
            case OperatorType.Multiply:
                SymbolToken(SymbolType.Star);
                break;
            case OperatorType.Divide:
                SymbolToken(SymbolType.ForwardSlash);
                break;
            case OperatorType.Equal:
                SymbolToken(SymbolType.Equals);
                SymbolToken(SymbolType.Equals);
                break;
            case OperatorType.NotEqual:
                SymbolToken(SymbolType.Exclamation);
                SymbolToken(SymbolType.Equals);
                break;
            case OperatorType.GreaterThan:
                SymbolToken(SymbolType.RightChevron);
                break;
            case OperatorType.LessThan:
                SymbolToken(SymbolType.LeftChevron);
                break;
            case OperatorType.GreaterThanOrEqual:
                SymbolToken(SymbolType.RightChevron);
                SymbolToken(SymbolType.Equals);
                break;
            case OperatorType.LessThanOrEqual:
                SymbolToken(SymbolType.LeftChevron);
                SymbolToken(SymbolType.Equals);
                break;
            case OperatorType.And:
                SymbolToken(SymbolType.Ampersand);
                SymbolToken(SymbolType.Ampersand);
                break;
            case OperatorType.Or:
                SymbolToken(SymbolType.VerticalBar);
                SymbolToken(SymbolType.VerticalBar);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    // Keyword
    //

    public KeywordToken KeywordToken(KeywordType type)
        => AddToken(new KeywordToken(type, CreateSourceReference()));

    // Named Parameters
    //

    public NamedParameter CreateNamedParameter(Type? type = null)
        => new(type ?? Type.Boolean, CreateIdentifierName());
    
    public NamedParameter NamedParameterTokens(Type? type = null)
    {
        var parameter = CreateNamedParameter(type);

        TypeTokens(parameter.Type);
        IdentifierToken(parameter.Name);

        return parameter;
    }
}

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

    public static void Literal(ref List<Token> tokens, Type type)
    {
        if (type == Thoth.Type.Integer)
        {
            tokens.Add(IntegerLiteral);
            return;
        }

        if (type == Thoth.Type.String)
        {
            tokens.Add(StringLiteral);
            return;
        }

        if (type == Thoth.Type.Boolean)
        {
            tokens.Add(BooleanLiteral);
            return;
        }

        if (type.Root == Thoth.BuiltinType.List)
        {
            tokens.Add(Symbol(SymbolType.LeftSquareBracket));
            tokens.Add(Symbol(SymbolType.RightSquareBracket));
            return;
        }

        throw new NotImplementedException();
    }

    public static NamedParameter NamedParameter(Type type)
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

    public static BuiltinTypeToken BuiltinType(BuiltinType type)
        => new(type, SourceReference);

    public static void Type(ref List<Token> tokens, Type type)
    {
        tokens.Add(BuiltinType(type.Root));

        if (type.HasParameters)
        {
            foreach (var parameter in type.Parameters)
            {
                Type(ref tokens, parameter);
            }
        }
    }
}
