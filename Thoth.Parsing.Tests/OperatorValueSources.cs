namespace Thoth.Parsing.Tests;

public static class OperatorValueSources
{
    public static IEnumerable<OperatorType> Operators
        => Enum.GetValues<OperatorType>();

    public static IEnumerable<OperatorType> Mathematical
        => Operators.Where(o => o.IsMathemeticalOperation());

    public static IEnumerable<OperatorType> Boolean
        => Operators.Where(o => o.IsBooleanOperation());

    public static IEnumerable<OperatorType> BinaryBoolean
        => Boolean.Where(o => !o.IsUnaryOperation());
}
