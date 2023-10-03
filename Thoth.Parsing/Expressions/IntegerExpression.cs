namespace Thoth.Parsing.Expressions;

public record IntegerExpression(long Value)
    : Expression(Thoth.Type.Integer)
{
    protected override string ArgumentsToString()
        => Value.ToString();
}
