namespace Thoth;

public enum OperatorType
{
    Add,
    Multiply,
    Subtract,
    Divide,
    Range,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Equal,
    NotEqual,
    Not,
    And,
    Or
}

public class InvalidOperationException(OperatorType operation, OperatorType? expected = null, string? message = null)
    : Exception(message ?? GenerateMessage(operation, expected))
{
    public readonly OperatorType Operation = operation;

    public readonly OperatorType? Expected = expected;

    private static string GenerateMessage(OperatorType operation, OperatorType? expected)
    {
        if (expected is not null) return $"Expected operation {expected} not {operation}.";

        return $"Unexpected operation {operation}.";
    }
}

public static class OperatorTypeExtensions
{
#region Attributes

    public static bool IsLogicalOperation(this OperatorType type)
    {
        return type switch
        {
            OperatorType.Not => true,
            OperatorType.And => true,
            OperatorType.Or  => true,

            _ => false,
        };
    }

    public static bool IsMathemeticalOperation(this OperatorType type)
    {
        return type switch
        {
            OperatorType.Add      => true,
            OperatorType.Multiply => true,
            OperatorType.Subtract => true,
            OperatorType.Divide   => true,

            _ => false,
        };
    }

    public static bool IsComparisonOperation(this OperatorType type)
    {
        return type switch
        {
            OperatorType.GreaterThan        => true,
            OperatorType.LessThan           => true,
            OperatorType.GreaterThanOrEqual => true,
            OperatorType.LessThanOrEqual    => true,
            OperatorType.Equal              => true,
            OperatorType.NotEqual           => true,

            _ => false,
        };
    }

    public static bool IsUnaryOperation(this OperatorType type)
    {
        return type switch
        {
            OperatorType.Not => true,

            _ => false,
        };
    }

#endregion

    public static string ToSourceString(this OperatorType type)
    {
        return type switch
        {
            OperatorType.Add                => "+",
            OperatorType.Multiply           => "*",
            OperatorType.Subtract           => "-",
            OperatorType.Divide             => "/",
            OperatorType.GreaterThan        => ">",
            OperatorType.LessThan           => "<",
            OperatorType.GreaterThanOrEqual => ">=",
            OperatorType.LessThanOrEqual    => "<=",
            OperatorType.Equal              => "==",
            OperatorType.NotEqual           => "!=",
            OperatorType.Range              => "..",
            OperatorType.Not                => "!",

            _ => throw new NotImplementedException()
        };
    }
}
