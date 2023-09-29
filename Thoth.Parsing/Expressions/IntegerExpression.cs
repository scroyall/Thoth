namespace Thoth.Parsing.Expressions;

public record IntegerExpression(long Value)
    : Expression(BasicType.Integer)
{
    protected override string ArgumentsToString()
    {
        return Value.ToString();
    }
}