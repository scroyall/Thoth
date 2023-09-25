namespace Thoth;

public enum BasicType
{
    Integer,
    Boolean,
    String
}

public class MismatchedTypeException(BasicType expected, BasicType actual)
    : Exception($"Type mismatch; expected {expected} not {actual}.");

public class UnresolvedTypeException()
    : Exception();

public static class BasicTypeExtensions
{
    public static BasicType CheckMatches(this BasicType type, BasicType other)
    {
        if (type.Matches(other)) return type;

        throw new MismatchedTypeException(other, type);
    }

    public static BasicType? CheckMatches(this BasicType? type, BasicType? other)
    {
        // Unresolved types matches any other resolved or unresolved type.
        if (type is null || other is null) return type;

        return CheckMatches(type.Resolved(), other.Resolved());
    }

    public static bool Matches(this BasicType type, BasicType other)
    {
        return other == type;
    }

    public static BasicType Resolved(this BasicType? type)
    {
        return type switch
        {
            { } resolved => resolved,
            _ => throw new UnresolvedTypeException()
        };
    }
}