namespace Thoth.Parsing.Expressions;

public record IndexExpression(Expression Indexable, Expression Index)
    : Expression
{
    protected override string ArgumentsToString()
        => $"{Indexable}[{Index}]";
}
