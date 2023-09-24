namespace Thoth.Parser.Expressions;

public class RangeExpression
    : BinaryExpression
{
    public Expression Start => Left;
    public Expression End => Right;

    public RangeExpression(Expression start, Expression end)
        : base(start, end)
    { }
}
