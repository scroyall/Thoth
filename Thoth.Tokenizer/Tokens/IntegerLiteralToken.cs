namespace Thoth.Tokenizer.Tokens;

public class IntegerLiteralToken
    : ValueToken<long>
{
    public IntegerLiteralToken(long value, SourceReference source)
        : base(value, source)
    { }
}
