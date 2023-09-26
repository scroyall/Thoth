namespace Thoth.Parsing.Expressions;

public class BooleanLiteralExpression(bool value)
    : Expression(BasicType.Boolean)
{
    public bool Value { get; } = value;

    protected override string ArgumentsToString()
    {
        return Value.ToString();
    }
}