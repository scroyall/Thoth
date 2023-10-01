namespace Thoth.Tokenization.Tokens;

public class TypeToken
    : ValueToken<IType>
{
    public IType Type => Value;

    public TypeToken(IType type, SourceReference source)
        : base(type, source)
    { }

    public override string ToString()
        => Type.ToString();
}