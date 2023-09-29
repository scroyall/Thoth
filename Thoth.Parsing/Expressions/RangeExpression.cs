namespace Thoth.Parsing.Expressions;

public record RangeExpression(Expression Start, Expression End)
    : BinaryExpression(BasicType.Integer, Start, End);