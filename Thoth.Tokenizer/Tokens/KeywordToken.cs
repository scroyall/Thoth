namespace Thoth.Tokenizer.Tokens;

public class KeywordToken
    : ValueToken<KeywordType>
{
    public KeywordType Type => Value;

    public KeywordToken(KeywordType type, SourceReference source)
        : base(type, source)
    { }

    public override string ToString()
    {
        return Type.ToString();
    }
}
