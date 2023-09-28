namespace Thoth.Tokenization;

public enum KeywordType
{
    Exit,
    Var,
    If,
    While,
    For,
    In,
    Assert,
    Print,
    Function
}

public static class KeywordExtensions
{
    public static string ToIdentifier(this KeywordType type)
    {
        return type.ToString().ToLower();
    }
}
