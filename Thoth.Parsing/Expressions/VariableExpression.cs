namespace Thoth.Parsing.Expressions;

public record VariableExpression(string Identifier)
    : Expression(Type: null)
{
    protected override string ArgumentsToString()
    {
        return Identifier;
    }
}