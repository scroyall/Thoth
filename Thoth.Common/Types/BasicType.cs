using System.Text;

namespace Thoth;

public record Type(BuiltinType Root, params Type[] Parameters)
{
#region Built-in Types

    public static Type Integer = new(BuiltinType.Int);

    public static Type Boolean = new(BuiltinType.Bool);

    public static Type String = new(BuiltinType.String);

    public static Type List(Type parameter) => new(BuiltinType.List, [parameter]);

#endregion

    public bool HasParameters => Parameters.Length > 0;

    public bool Matches(Type other)
    {
        return Equals(other);
    }

    public Type Match(Type other)
    {
        if (!Matches(other)) throw new MismatchedTypeException(other, this);

        return this;
    }

    public BuiltinType MatchRoot(BuiltinType other)
    {
        if (Root != other) throw new MismatchedTypeException(other, Root);

        return Root;
    }

    public Type MatchList(Type? memberType = null)
    {
        // Check the root type is a list.
        MatchRoot(BuiltinType.List);

        // Check there is exactly one parameter, the list's member type.
        if (Parameters.Length != 1) throw new InvalidParameterCountException(1, Parameters.Length);

        // If the member type was specified, match that too.
        if (memberType is not null)
        {
            var actual = Parameters[0] ?? throw new NullReferenceException();
            actual.Match(memberType);
        }

        return this;
    }

    public override string ToString()
    {
        StringBuilder builder = new(Root.ToString());

        if (HasParameters)
        {
            builder.Append('<');

            var parametersAsStrings = Parameters.Select(p => p.ToString());
            builder.Append(string.Join(", ", parametersAsStrings));

            builder.Append('>');
        }

        return builder.ToString();
    }

    public override int GetHashCode()
    {
        var hash = Root.GetHashCode();

        foreach (var parameter in Parameters)
        {
            hash = HashCode.Combine(hash, parameter.GetHashCode());
        }

        return hash;
    }

    public virtual bool Equals(Type? obj)
    {
        if (obj is not Type type) return false;

        if (type.Root != Root) return false;
        if (type.Parameters.Length != Parameters.Length) return false;

        for (int i = 0; i < Parameters.Length; i++)
        {
            if (type.Parameters[i] != Parameters[i]) return false;
        }

        return true;
    }
}
