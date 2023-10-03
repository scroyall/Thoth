namespace Thoth.Parsing.Expressions;

public record StringExpression(int Index)
    : Expression(Thoth.Type.String)
{
    protected override string ArgumentsToString()
        => Index.ToString();
}
