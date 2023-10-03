namespace Thoth.Tokenization.Tokens;

public class BuiltinTypeToken(BuiltinType Type, SourceReference Source)
    : ValueToken<BuiltinType>(Type, Source)
{
    public BuiltinType Type => Value;

    public override string ToString()
        => Type.ToString();
}
