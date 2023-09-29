namespace Thoth.Parsing.Expressions;

public record FunctionCallExpression(string Name)
    : Expression(Type: null)
    , IFunctionCall;