using System.Text;

using Thoth.Tokenization.Tokens;
using Thoth.Utils;

namespace Thoth.Tokenization;

public class UnexpectedCharacterException
    : Exception
{
    public UnexpectedCharacterException(char character)
        : base($"Unexpected character '{character}'.")
    { }
}

public class UnexpectedEndOfInputException
    : Exception
{ }

public class Tokenizer
{
#region Keywords

    private const string ExitKeyword = "exit";
    private const string LetKeyword = "let";
    private const string IfKeyword = "if";

    #endregion

    private AtomStack<char?>? _input = null;

    private AtomStack<char?> Input => _input ?? throw new NullReferenceException();

    private List<Token> Tokens { get; } = [];

    private List<string> Strings { get; } = [];

    private int Line { get; set; }

    private int Column { get; set; }

    public TokenizedProgram Tokenize(string input)
    {
        var characters = input.Select(character => (char?) character).ToList();
        _input = new AtomStack<char?>(characters);

        Tokens.Clear();
        Strings.Clear();

        Input.Reset();
        Line = 1;
        Column = 1;

        while (Input.Peek() is { } character)
        {
            // Consume whitespace.
            if (char.IsWhiteSpace(character))
            {
                ConsumeWhitespace();
                continue;
            }

            if (character == '#')
            {
                ConsumeComment();
                continue;
            }

            Tokens.Add(ParseToken());
        }

        return new TokenizedProgram(Tokens, Strings);
    }

    private Token ParseToken()
    {
        if (Input.Peek() is not { } character)
        {
            throw new UnexpectedEndOfInputException();
        }

        if (char.IsLetter(character)) return ParseWord();
        if (char.IsNumber(character)) return ParseInteger();
        if (SymbolExtensions.TryParse(character, out var symbol)) return ParseSymbol(symbol);

        throw new UnexpectedCharacterException(character);
    }

    /// <summary>
    /// Parse a word into a keyword or identifier token.
    /// </summary>
    private Token ParseWord()
    {
        var buffer = new StringBuilder();

        while (Input.Peek() is { } character)
        {
            // Check the character continues the keyword.
            if (!char.IsLetterOrDigit(character))
            {
                break;
            }

            // Add the character to the buffer and consume it.
            buffer.Append(character);
            Consume();
        }

        var word = buffer.ToString();

        // Check for keywords.
        if (Enum.TryParse(word, true, out KeywordType keyword))
        {
            return new KeywordToken(keyword, SourceReference.OffsetBy(buffer.Length));
        }

        // Check for types.
        if (BasicTypeExtensions.TryParse(word, out var type))
        {
            return new TypeToken(type, SourceReference.OffsetBy(buffer.Length));
        }

        // Everything that's not a keyword must be an identifier.
        return new IdentifierToken(word, SourceReference.OffsetBy(buffer.Length));
    }

    /// <summary>
    /// Parse an integer literal.
    /// </summary>
    private IntegerLiteralToken ParseInteger()
    {
        var buffer = new StringBuilder();

        bool negative = TryConsume((char) SymbolType.Minus); // Negative if the first character is a minus.

        while (Input.Peek() is { } character)
        {
            // Check the character continues the integer.
            if (!char.IsNumber(character))
            {
                break;
            }

            // Add the character to the buffer and consume it.
            buffer.Append(character);
            Consume();
        }

        var valueAsString = buffer.ToString();
        if (long.TryParse(valueAsString, out var value))
        {
            if (negative) value *= -1;

            return new IntegerLiteralToken(value, SourceReference.OffsetBy(valueAsString.Length));
        }

        throw new Exception($"Failed to parse integer '{valueAsString}'.");
    }

    private Token ParseSymbol(SymbolType symbol)
    {
        switch (symbol)
        {
            case SymbolType.DoubleQuote: // Double quote starts a string literal.
                return ParseString();
            case SymbolType.Minus: // Minus might start an integer.
                if (Input.Peek(1) is { } next && char.IsNumber(next))
                {
                    return ParseInteger();
                }
                break;
        }

        Consume();
        return new SymbolToken(symbol, SourceReference.OffsetBy(1));
    }

    private StringLiteralToken ParseString()
    {
        Consume('"');

        var buffer = new StringBuilder();
        while (Input.Peek() is { } character)
        {
            // Double quote terminates the string.
            if (character == '"') break;

            buffer.Append(character);
            Consume();
        }

        Consume('"');

        var index = CreateString(buffer.ToString());
        return new StringLiteralToken(index, SourceReference.OffsetBy(buffer.Length));
    }

    /// <summary>
    /// Consume whitespace until no more is found.
    /// </summary>
    private void ConsumeWhitespace()
    {
        // Consume until the next character is not whitespace.
        while (Input.Peek() is { } character && char.IsWhiteSpace(character))
        {
            switch (character)
            {
                case ' ': // Space
                case '\t': // Tab
                    Consume();
                    break;
                case '\n': // Line Feed
                    Consume();
                    IncrementLine();
                    break;
                case '\r': // Carriage Return
                    Consume();

                    // Also consume a line feed if present, so a CRLF is only one new line.
                    TryConsume('\n');

                    IncrementLine();
                    break;
                default:
                    throw new UnexpectedCharacterException(character);
            }
        }
    }

    private void ConsumeComment()
    {
        while (Input.Peek() is { } character)
        {
            switch (character)
            {
                case '\r':
                case '\n':
                    return;
            }

            Consume();
        }
    }

    private bool TryConsume(char symbol)
    {
        if (Input.Peek() != symbol) return false;

        Consume();
        return true;
    }

    /// <summary>
    /// Consume a character from the input.
    /// </summary>
    private void Consume()
    {
        if (Input.Consume() is null)
        {
            throw new UnexpectedEndOfInputException();
        }

        Column++;
    }

    private void Consume(char character)
    {
        if (Input.Peek() is not { } peek) throw new UnexpectedEndOfInputException();
        if (peek != character) throw new UnexpectedCharacterException(peek);

        Input.Consume();
    }

    private SourceReference SourceReference => new(Line, Column);

    private void IncrementLine()
    {
        Line++;
        Column = 1;
    }

    #region Strings

    private int CreateString(string value)
    {
        if (Strings.Contains(value))
        {
            return Strings.IndexOf(value);
        }
        
        Strings.Add(value);
        return Strings.Count - 1;
    }

    #endregion
}
