namespace Thoth.Parsing.Expressions;

public record ListLiteralExpression(IReadOnlyList<Expression> values)
    : Expression(Type: null)
{
    public IReadOnlyList<Expression> Values = CheckValues(values);

    public static IReadOnlyList<Expression> CheckValues(IReadOnlyList<Expression> values)
    {
        BasicType? type = null;
        foreach (var value in values)
        {
            type = value.Type.CheckMatches(type);
        }

        return values;
    }

    protected override string ArgumentsToString()
        => string.Join(", ", Values);
}
