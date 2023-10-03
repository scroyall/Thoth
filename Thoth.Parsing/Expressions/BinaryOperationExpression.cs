namespace Thoth.Parsing.Expressions;

public record BinaryOperationExpression(OperatorType Operation, Expression Left, Expression Right)
    : BinaryExpression(Left, Right)
{
    protected override string ArgumentsToString()
        => $"{Left} {Operation} {Right}";
}
