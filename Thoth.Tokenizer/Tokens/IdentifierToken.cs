namespace Thoth.Tokenizer.Tokens;

public class IdentifierToken
    : ValueToken<string>
{
    public string Name => Value;

    public IdentifierToken(string value, SourceReference source)
        : base(value, source)
    { }
}
