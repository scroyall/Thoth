namespace Thoth.Parsing.Expressions;

public record BinaryOperationExpression(BasicType? Type, OperatorType Operation, Expression Left, Expression Right)
    : BinaryExpression(Type, Left, Right)
{
    protected override string ArgumentsToString()
    {
        return $"{Left} {Operation} {Right}";
    }
}