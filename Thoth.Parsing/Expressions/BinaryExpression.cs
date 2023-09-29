namespace Thoth.Parsing.Expressions;

public record BinaryExpression(BasicType? Type, Expression Left, Expression Right)
    : Expression(Type);