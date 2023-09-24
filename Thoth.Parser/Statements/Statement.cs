namespace Thoth.Parser.Statements;

using Tokenizer;

public class Statement
{
    public readonly SourceReference Source;

    public Statement(SourceReference source)
    {
        Source = source;
    }

    protected virtual string ArgumentsToString() => "";

    public override string ToString()
    {
        return $"{GetType().Name}({ArgumentsToString()})[{Source}]";
    }
}
