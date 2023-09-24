namespace Thoth.Tokenization;

public class Token
{
    public readonly SourceReference Source;

    public Token(SourceReference source)
    {
        Source = source;
    }

    protected virtual string ArgumentsToString()
    {
        return "";
    }

    public override string ToString()
    {
        return $"{GetType().Name}({ArgumentsToString()})[{Source}]";
    }
}
