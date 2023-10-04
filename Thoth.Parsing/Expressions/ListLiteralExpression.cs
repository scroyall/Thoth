namespace Thoth.Parsing.Expressions;

public record ListLiteralExpression(Type MemberType, IReadOnlyList<Expression> Values)
    : Expression(Type.List(MemberType))
{
    protected override string ArgumentsToString()
        => string.Join(", ", Values);
}
