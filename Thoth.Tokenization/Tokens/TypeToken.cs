namespace Thoth.Tokenization.Tokens;

public class TypeToken
    : ValueToken<BasicType>
{
    public BasicType Type => Value;

    public TypeToken(BasicType type, SourceReference source)
        : base(type, source)
    { }

    public override string ToString()
    {
        return Type.ToString();
    }
}