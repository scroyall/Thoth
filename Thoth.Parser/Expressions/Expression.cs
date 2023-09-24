namespace Thoth.Parser.Expressions;

public class Expression
{
    public override string ToString()
    {
        return $"{GetType().Name}({ArgumentsToString()})";
    }

    protected virtual string ArgumentsToString() => string.Empty;
}
