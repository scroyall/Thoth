namespace Thoth.Parser.Expressions;

public class VariableExpression
    : Expression
{
    public readonly string Identifier;

    public VariableExpression(string identifier)
    {
        Identifier = identifier;
    }

    protected override string ArgumentsToString()
    {
        return Identifier;
    }
}
