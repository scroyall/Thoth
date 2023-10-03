namespace Thoth.Parsing.Expressions;

public record ListLiteralExpression(IReadOnlyList<Expression> Values)
    : Expression
{
    protected override string ArgumentsToString()
        => string.Join(", ", Values);
}
