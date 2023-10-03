namespace Thoth.Parsing.Expressions;

public record FunctionCallExpression(string Name, IReadOnlyList<Expression> Parameters)
    : Expression
    , IFunctionCall;
