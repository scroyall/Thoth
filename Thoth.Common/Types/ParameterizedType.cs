namespace Thoth;

public record ResolvedParameterizedType(IResolvedType Outer, IResolvedType Parameter)
    : IResolvedType
{
    public bool IsResolved() => true;

    public IResolvedType Resolve() => this;

    public bool Matches(IType other) => Equals(this, other);

    public IType Match(IType other)
    {
        if (!Matches(other)) throw new MismatchedTypeException(other, this);

        return this;
    }
}

public record ParameterizedType(IResolvedType Outer, IType Parameter)
    : IType
{
    public bool IsResolved() => Parameter.IsResolved();

    public IResolvedType Resolve() => new ResolvedParameterizedType(Outer, Parameter.Resolve());

    public bool Matches(IType other)
    {
        // Every type matches a completely unresolved type.
        if (ReferenceEquals(other, BasicType.Unresolved)) return true;

        // Unresolved parameterized types always match any other parameterized type with a matching outer type.
        if (!IsResolved() && other is ParameterizedType p && p.Outer.Matches(Outer)) return true;

        // Basic types match parameterized types with a matching outer type.
        if (other is BasicType b && b.Matches(Outer)) return true;

        return Equals(this, other);
    }

    public IType Match(IType other)
    {
        if (!Matches(other)) throw new MismatchedTypeException(other, this);

        return (IsResolved() || !other.IsResolved()) ? this : other;
    }
}
