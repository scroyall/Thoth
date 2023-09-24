namespace Thoth.Tokenization;

public struct SourceReference(int line, int column)
{
    public readonly int Line = line;
    public readonly int Column = column;

    public SourceReference OffsetBy(int value)
    {
        return new SourceReference(Line, Column - value);
    }

    public override string ToString()
    {
        return $"{Line}:{Column}";
    }
}