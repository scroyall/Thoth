namespace Thoth.Parsing.Expressions;

public record Expression(IType Type)
{
    public override string ToString()
    {
        return $"{GetType().Name}[{Type}]({ArgumentsToString()})";
    }

    protected virtual string ArgumentsToString() => string.Empty;
}

public static class ExpressionExtensions
{
    public static IType FindMostExactType(this IEnumerable<Expression> expressions)
    {
        IType type = BasicType.Unresolved;

        foreach (Expression expression in expressions)
        {
            type = expression.Type.Match(type);
        }

        return type;
    }
}
