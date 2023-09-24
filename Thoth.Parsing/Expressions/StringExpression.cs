namespace Thoth.Parsing.Expressions;

public class StringExpression
    : Expression
{
    public readonly int Index;

    public StringExpression(int index)
        : base(BasicType.String)
    {
        Index = index;
    }

    protected override string ArgumentsToString()
    {
        return Index.ToString();
    }
}