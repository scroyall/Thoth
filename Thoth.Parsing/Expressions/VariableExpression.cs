namespace Thoth.Parsing.Expressions;

public record VariableExpression(string Identifier)
    : Expression(BasicType.Unresolved)
{
    protected override string ArgumentsToString()
    {
        return Identifier;
    }
}