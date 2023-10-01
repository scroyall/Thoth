namespace Thoth.Tokenization;

public enum KeywordType
{
    Exit,
    If,
    While,
    For,
    In,
    Assert,
    Print,
    Function,
    Return
}

public static class KeywordExtensions
{
    public static string ToIdentifier(this KeywordType type)
    {
        return type.ToString().ToLower();
    }
}
