namespace Thoth.Parsing.Expressions;

public class VariableExpression
    : Expression
{
    public readonly string Identifier;

    public VariableExpression(string identifier)
        : base(null)
    {
        Identifier = identifier;
    }

    protected override string ArgumentsToString()
    {
        return Identifier;
    }
}
