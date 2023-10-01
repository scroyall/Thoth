namespace Thoth;

public record ResolvedBasicType(string source)
    : BasicType(source), IResolvedType
{
    public new static IEnumerable<ResolvedBasicType> Values => [Integer, Boolean, String, List];

    public override string ToString()
        => SourceString;
}

public record BasicType
    : IType
{
    public readonly string SourceString;

    public static BasicType Unresolved = new("var");
    public static ResolvedBasicType Integer = new("int");
    public static ResolvedBasicType Boolean = new("bool");
    public static ResolvedBasicType String = new("string");
    public static ResolvedBasicType List = new("list");

    public static IEnumerable<BasicType> Values => Enumerable.Concat(ResolvedBasicType.Values, [Unresolved]);

    internal BasicType(string source)
    {
        SourceString = source;
    }

    public bool IsResolved()
    {
        if (ReferenceEquals(this, List)) return false;

        return !ReferenceEquals(this, Unresolved);
    }

    public IType Match(IType other)
    {
        if (!Matches(other)) throw new MismatchedTypeException(other, this);

        // Prefer to return a resolved type, if possible.
        return IsResolved() ? this : other;
    }

    public bool Matches(IType other)
    {
        // Completely unresolved types match every type.
        if (ReferenceEquals(this, Unresolved) || ReferenceEquals(other, Unresolved)) return true;

        // Resolved basic types only match themselves.
        return ReferenceEquals(this, other);
    }

    public IResolvedType Resolve()
    {
        return (this as ResolvedBasicType) ?? throw new UnresolvedTypeException();
    }

    public override string ToString()
        => SourceString;

    public static BasicType? TryParse(string source)
    {
        foreach (var type in Values)
        {
            if (source == type.SourceString) return type;
        }

        return null;
    }
}
