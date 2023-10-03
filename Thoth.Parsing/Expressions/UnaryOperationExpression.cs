namespace Thoth.Parsing.Expressions;

public record UnaryOperationExpression(OperatorType Operation, Expression Value)
    : Expression
{
    protected override string ArgumentsToString()
        => $"{Operation} {Value}";
}
