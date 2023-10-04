namespace Thoth.Tests;

public class TypeValueSources
{
    public static IEnumerable<Type> Builtin
        => [
            Type.Integer,
            Type.Boolean,
            Type.String
        ];

    public static IEnumerable<Type> Lists
        => Builtin.Select(t => Type.List(t));

    public static IEnumerable<Type> All
        => Enumerable.Concat(
            Builtin,
            Lists
        );
}
