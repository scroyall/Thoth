namespace Thoth.Parsing.Expressions;

public record BooleanLiteralExpression(bool Value)
    : Expression(Thoth.Type.Boolean)
{
    protected override string ArgumentsToString()
        => Value.ToString();
}
