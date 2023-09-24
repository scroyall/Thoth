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

    private readonly AtomStack<char?> _input;

    private readonly List<Token> _tokens = new();
    private readonly List<string> _strings = new();

    private int _line;
    private int _column;

    public Tokenizer(string input)
    {
        var characters = input.Select(character => (char?) character).ToList();
        _input = new AtomStack<char?>(characters);
    }

    public TokenizedProgram Tokenize()
    {
        _tokens.Clear();
        _strings.Clear();

        _input.Reset();
        _line = 1;
        _column = 1;

        while (_input.Peek() is { } character)
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

            _tokens.Add(ParseToken());
        }

        return new TokenizedProgram(_tokens, _strings);
    }

    private Token ParseToken()
    {
        if (_input.Peek() is not { } character)
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

        while (_input.Peek() is { } character)
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
        if (Enum.TryParse(word, true, out KeywordType type))
        {
            return new KeywordToken(type, SourceReference.OffsetBy(buffer.Length));
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

        while (_input.Peek() is { } character)
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
                if (_input.Peek(1) is { } next && char.IsNumber(next))
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
        while (_input.Peek() is { } character)
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
        while (_input.Peek() is { } character && char.IsWhiteSpace(character))
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
        while (_input.Peek() is { } character)
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
        if (_input.Peek() != symbol) return false;

        Consume();
        return true;
    }

    /// <summary>
    /// Consume a character from the input.
    /// </summary>
    private void Consume()
    {
        if (_input.Consume() is null)
        {
            throw new UnexpectedEndOfInputException();
        }

        _column++;
    }

    private void Consume(char character)
    {
        if (_input.Peek() is not { } peek) throw new UnexpectedEndOfInputException();
        if (peek != character) throw new UnexpectedCharacterException(peek);

        _input.Consume();
    }

    private SourceReference SourceReference => new(_line, _column);

    private void IncrementLine()
    {
        _line++;
        _column = 1;
    }

    #region Strings

    private int CreateString(string value)
    {
        if (_strings.Contains(value))
        {
            return _strings.IndexOf(value);
        }
        
        _strings.Add(value);
        return _strings.Count - 1;
    }

    #endregion
}
