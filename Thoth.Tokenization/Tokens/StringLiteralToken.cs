namespace Thoth.Tokenization.Tokens;

public class StringLiteralToken
    : ValueToken<int>
{
    public int Index => Value;

    public StringLiteralToken(int index, SourceReference source)
        : base(index, source)
    { }

    public override string ToString()
    {
        return $"String:{Index}";
    }
}