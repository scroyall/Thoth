namespace Thoth.Parser.Expressions;

public class BinaryOperationExpression
    : Expression
{
    public readonly OperatorType Operation;
    public readonly Expression Left;
    public readonly Expression Right;

    public BinaryOperationExpression(OperatorType operation, Expression left, Expression right)
    {
        Operation = operation;
        Left = left;
        Right = right;
    }
    
    protected override string ArgumentsToString()
    {
        return $"Left={Left}, Right={Right}";
    }
}
