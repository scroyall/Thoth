namespace Thoth.Tokenization.Tokens;

public class IdentifierToken
    : ValueToken<string>
{
    public string Name => Value;

    public IdentifierToken(string name, SourceReference source)
        : base(name, source)
    { }
}
