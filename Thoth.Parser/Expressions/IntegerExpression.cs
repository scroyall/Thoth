namespace Thoth.Parser.Expressions;

public class IntegerExpression
    : Expression
{
    public readonly long Value;

    public IntegerExpression(long value)
    {
        Value = value;
    }

    protected override string ArgumentsToString()
    {
        return Value.ToString();
    }
}
