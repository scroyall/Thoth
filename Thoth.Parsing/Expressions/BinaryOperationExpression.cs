namespace Thoth.Parsing.Expressions;

public class BinaryOperationExpression
    : BinaryExpression
{
    public readonly OperatorType Operation;

    public BinaryOperationExpression(BasicType? type, OperatorType operation, Expression left, Expression right)
        : base(type, left, right)
    {
        Operation = operation;
    }
    
    protected override string ArgumentsToString()
    {
        return $"{Left} {Operation} {Right}";
    }
}