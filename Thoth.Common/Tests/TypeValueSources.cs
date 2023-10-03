namespace Thoth.Tests;

public class TypeValueSources
{
    public static IEnumerable<Type> All
        => [
            Type.Integer,
            Type.Boolean,
            Type.String,
            Type.List(Type.Integer),
            Type.List(Type.Boolean),
            Type.List(Type.String),
            Type.List(Type.List(Type.Integer)),
            Type.List(Type.List(Type.Boolean)),
            Type.List(Type.List(Type.String)),
        ];
}
