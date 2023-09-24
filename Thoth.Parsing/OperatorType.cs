namespace Thoth.Parsing;

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
    NotEqual
}

public static class OperatorTypeExtensions
{
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

    public static bool IsBooleanOperation(this OperatorType type)
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

            _ => throw new NotImplementedException()
        };
    }
}
