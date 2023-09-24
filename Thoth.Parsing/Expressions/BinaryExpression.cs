namespace Thoth.Parsing.Expressions;

public class BinaryExpression
    : Expression
{
    public readonly Expression Left;
    public readonly Expression Right;

    public BinaryExpression(BasicType? type, Expression left, Expression right)
        : base(type)
    {
        Left = left;
        Right = right;
    }
    
    protected override string ArgumentsToString()
    {
        return $"Left={Left}, Right={Right}";
    }
}
