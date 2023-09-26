namespace Thoth.Parsing.Expressions;

public class BinaryOperationExpression(BasicType? type, OperatorType operation, Expression left, Expression right)
    : BinaryExpression(type, left, right)
{
    public OperatorType Operation { get; } = operation;

    protected override string ArgumentsToString()
    {
        return $"{Left} {Operation} {Right}";
    }
}