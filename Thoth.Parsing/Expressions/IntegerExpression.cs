namespace Thoth.Parsing.Expressions;

public class IntegerExpression
    : Expression
{
    public readonly long Value;

    public IntegerExpression(long value)
        : base(BasicType.Integer)
    {
        Value = value;
    }

    protected override string ArgumentsToString()
    {
        return Value.ToString();
    }
}
