namespace Thoth.Parsing.Expressions;

public record StringExpression(int Index)
    : Expression(BasicType.String)
{
    protected override string ArgumentsToString()
    {
        return Index.ToString();
    }
}