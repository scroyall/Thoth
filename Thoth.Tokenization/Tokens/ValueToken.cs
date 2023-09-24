namespace Thoth.Tokenization.Tokens;

public abstract class ValueToken<TValue>
    : Token
    where TValue : notnull
{
    public TValue Value { get; }

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
