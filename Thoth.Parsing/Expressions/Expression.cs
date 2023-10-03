namespace Thoth.Parsing.Expressions;

public record Expression(Type? Type = null)
{
    public override string ToString()
    {
        return $"{GetType().Name}[{Type}]({ArgumentsToString()})";
    }

    protected virtual string ArgumentsToString() => string.Empty;
}
