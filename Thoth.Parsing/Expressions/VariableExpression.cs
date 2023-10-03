namespace Thoth.Parsing.Expressions;

public record VariableExpression(string Identifier)
    : Expression
{
    protected override string ArgumentsToString()
        => Identifier;
}
