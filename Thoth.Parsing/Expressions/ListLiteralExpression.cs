namespace Thoth.Parsing.Expressions;

public record ListLiteralExpression(IReadOnlyList<Expression> Values)
    : Expression(
        Type: new ParameterizedType(BasicType.List, Values.FindMostExactType()))
{
    protected override string ArgumentsToString()
        => string.Join(", ", Values);
}
