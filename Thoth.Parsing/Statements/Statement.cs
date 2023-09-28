namespace Thoth.Parsing.Statements;

using Tokenization;

public record Statement(SourceReference Source)
{
    protected virtual string ArgumentsToString() => "";

    public override string ToString()
    {
        return $"{GetType().Name}({ArgumentsToString()})[{Source}]";
    }
}