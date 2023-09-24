namespace Thoth.Parser.Expressions;

public class StringExpression
    : Expression
{
    public readonly int Index;

    public StringExpression(int index)
    {
        Index = index;
    }

    protected override string ArgumentsToString()
    {
        return Index.ToString();
    }
}