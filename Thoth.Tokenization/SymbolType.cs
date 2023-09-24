namespace Thoth.Tokenization;

public enum SymbolType
{
    Semicolon = ';',
    LeftParenthesis = '(',
    RightParenthesis = ')',
    Equals = '=',
    Plus = '+',
    Star = '*',
    Minus = '-',
    ForwardSlash = '/',
    LeftBrace = '{',
    RightBrace = '}',
    Hash = '#',
    Dot = '.',
    LeftChevron = '<',
    RightChevron = '>',
    Exclamation = '!',
    DoubleQuote = '"'
}

public static class SymbolExtensions
{
    public static bool TryParse(char character, out SymbolType type)
    {
        if (Enum.IsDefined(typeof(SymbolType), (int) character))
        {
            type = (SymbolType) character;
            return true;
        }

        type = default; 
        return false;
    }

    public static char ToCharacter(this SymbolType symbol) => (char) symbol;
}
