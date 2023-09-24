namespace Thoth.Parser.Expressions;

public class BinaryExpression
    : Expression
{
    public readonly Expression Left;
    public readonly Expression Right;

    public BinaryExpression(Expression left, Expression right)
    {
        Left = left;
        Right = right;
    }
    
    protected override string ArgumentsToString()
    {
        return $"Left={Left}, Right={Right}";
    }
}
