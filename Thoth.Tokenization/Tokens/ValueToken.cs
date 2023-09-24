namespace Thoth.Tokenization.Tokens;

public abstract class ValueToken<TValue>
    : Token
    where TValue : notnull
{
    public readonly TValue Value;

    protected ValueToken(TValue value, SourceReference source)
        : base(source)
    {
        Value = value;
    }

    protected override string ArgumentsToString()
    {
        return Value.ToString() ?? "null";
    }
}
