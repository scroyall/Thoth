namespace Thoth.Parsing.Expressions;

public class Expression(BasicType? type)
{
    public readonly BasicType? Type = type;

    public override string ToString()
    {
        return $"{GetType().Name}[{Type}]({ArgumentsToString()})";
    }

    protected virtual string ArgumentsToString() => string.Empty;
}
