namespace Thoth.Parsing.Expressions;

public class UnaryOperationExpression(BasicType? type, OperatorType operation, Expression value)
    : Expression(type)
{
    public OperatorType Operation { get; } = CheckOperation(operation);

    public Expression Value { get; } = CheckValue(value, type);

    private static OperatorType CheckOperation(OperatorType operation)
    {
        if (!operation.IsUnaryOperation()) throw new ArgumentException($"Expected unary operation not {operation}.");

        return operation;
    }

    private static Expression CheckValue(Expression value, BasicType? type)
    {
        value.Type.CheckMatches(type);

        return value;
    }

    protected override string ArgumentsToString()
    {
        return $"{Operation} {Value}";
    }
}