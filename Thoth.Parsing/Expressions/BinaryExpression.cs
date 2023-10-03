namespace Thoth.Parsing.Expressions;

public record BinaryExpression(Expression Left, Expression Right)
    : Expression
{
    protected override string ArgumentsToString()
        => $"{Left}, {Right}";
}
