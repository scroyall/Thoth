namespace Thoth.Parsing.Expressions;

public record UnaryOperationExpression(IType type, OperatorType operation, Expression value)
    : Expression(value.Type.Match(type))
{
    public OperatorType Operation { get; } = CheckOperation(operation);

    public Expression Value { get; } = value;

    private static OperatorType CheckOperation(OperatorType operation)
    {
        if (!operation.IsUnaryOperation()) throw new ArgumentException($"Expected unary operation not {operation}.");

        return operation;
    }

    protected override string ArgumentsToString()
    {
        return $"{Operation} {Value}";
    }
}