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

    /// <summary>
    /// Check one possibly unresolved type matches another possibly unresolved type.
    /// </summary>
    public static BasicType? CheckMatches(this BasicType? type, BasicType? other)
    {
        // Unresolved types always match any other resolved or unresolved type.
        if (type is null || other is null) return type;

        return CheckMatches(type.Resolved(), other.Resolved());
    }

    /// <summary>
    /// Check a type matches another, possibly unresolved, type.
    /// </summary>
    public static BasicType CheckMatches(this BasicType type, BasicType? other)
    {
        // Resolved types always match unresolved types.
        if (other is null) return type;

        return CheckMatches(type, other.Resolved());
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

    public static string ToSourceString(this BasicType type)
    {
        return type switch
        {
            BasicType.Integer => "int",
            BasicType.Boolean => "bool",
            BasicType.String  => "string",
            _ => throw new NotImplementedException()
        };
    }

    public static bool TryParse(string name, out BasicType type)
    {
        BasicType? match = name switch
        {
            "int"    => BasicType.Integer,
            "bool"   => BasicType.Boolean,
            "string" => BasicType.String,
            _        => null,
        };

        type = match ?? default;
        return match is not null;
    }
}