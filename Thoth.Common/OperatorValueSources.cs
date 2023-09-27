namespace Thoth.Tests;

public static class OperatorValueSources
{
    public static IEnumerable<OperatorType> Operators
        => Enum.GetValues<OperatorType>();

    public static IEnumerable<OperatorType> Mathematical
        => Operators.Where(o => o.IsMathemeticalOperation());

    public static IEnumerable<OperatorType> Comparison
        => Operators.Where(o => o.IsComparisonOperation());

    public static IEnumerable<OperatorType> Logical
        => Operators.Where(o => o.IsLogicalOperation());

    public static IEnumerable<OperatorType> BinaryLogical
        => Logical.Where(o => !o.IsUnaryOperation());
}
