namespace Thoth.Parsing.Expressions;

public record BinaryExpression(IType Type, Expression Left, Expression Right)
    : Expression(Type);