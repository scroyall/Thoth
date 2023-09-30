namespace Thoth.Parsing.Expressions;

public record Expression(BasicType? Type)
{
    public override string ToString()
    {
        return $"{GetType().Name}[{Type}]({ArgumentsToString()})";
    }

    protected virtual string ArgumentsToString() => string.Empty;
}

public static class ExpressionExtensions
{
    public static BasicType? FindMostExactType(this IEnumerable<Expression> expressions)
    {
        BasicType? type = null;

        foreach (var expression in expressions)
        {
            type = expression.Type.CheckMatches(type);
        }

        return type;
    }
}
