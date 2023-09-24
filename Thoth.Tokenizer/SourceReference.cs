namespace Thoth.Tokenizer;

public struct SourceReference
{
    public readonly int Line;
    public readonly int Column;

    public SourceReference(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public SourceReference OffsetBy(int value)
    {
        return new SourceReference(Line, Column - value);
    }

    public override string ToString()
    {
        return $"{Line}:{Column}";
    }
}
