namespace Thoth.Tokenization.Tokens;

public class SymbolToken
    : ValueToken<SymbolType>
{
    public SymbolType Type => Value;

    public SymbolToken(SymbolType type, SourceReference source)
        : base(type, source)
    { }

    public override string ToString()
    {
        return Type.ToCharacter().ToString();
    }
}
