namespace Thoth.Parsing.Expressions;

public record BooleanLiteralExpression(bool Value)
    : Expression(BasicType.Boolean)
{
    protected override string ArgumentsToString()
    {
        return Value.ToString();
    }
}