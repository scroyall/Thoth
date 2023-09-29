namespace Thoth.Parsing.Expressions;

public record Expression(BasicType? Type)
{
    public override string ToString()
    {
        return $"{GetType().Name}[{Type}]({ArgumentsToString()})";
    }

    protected virtual string ArgumentsToString() => string.Empty;
}