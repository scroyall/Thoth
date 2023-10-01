namespace Thoth;

public interface IResolvedType
    : IType
{
    new bool IsResolved() => true;

    new IResolvedType Resolve() => this;

    new IResolvedType Match(IType other)
    {
        if (!Matches(other)) throw new MismatchedTypeException(other, this);

        return this;
    }
}

public interface IType
{
    bool IsResolved();

    IResolvedType Resolve();

    bool Matches(IType other);

    IType Match(IType other);

    string ToString();
}
