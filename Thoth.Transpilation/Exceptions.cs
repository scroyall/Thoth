using Thoth.Parsing;
using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;

namespace Thoth.Transpilation;

public class UnexpectedStatementException(Statement statement, string? message = null)
    : Exception(message ?? $"Unexpected statement '{statement}'.");

public class MissingStatementException<TStatement>(string? message = null)
    : Exception(message ?? $"Missing {typeof(TStatement).Name} statement.")
    where TStatement : Statement;

public class MissingReturnStatementException(Type? type)
    : MissingStatementException<ReturnStatement>(
        type is null ? "Missing return statement." : $"Missing return statement of type {type}.");

public class UnexpectedExpressionException(Expression expression)
    : Exception($"Unexpected expression {expression}.")
{
    public readonly Expression Expression = expression;
}

public class MissingExpressionException(Type type)
    : Exception($"Missing expression of type {type}.");

public class UndefinedVariableException(string identifier)
    : Exception($"Undefined variable '{identifier}'.");

public class UndefinedFunctionException(string identifier)
    : Exception($"Undefined function '{identifier}'.");

public class InvalidFunctionException(string Message)
    : Exception(Message);

public class InvalidParameterCountException(string identifier, int count, int expected)
    : Exception($"Invalid parameter count of {count} for '{identifier}' which expects {expected}.");

public class MultiplyDefinedVariableException(string identifier)
    : Exception($"Variable '{identifier}' is already defined.");

public class UnexpectedOperationException(OperatorType operation, OperatorType? expected = null, string? message = null)
    : Exception(message ?? GenerateMessage(operation, expected))
{
    public readonly OperatorType Operation = operation;

    public readonly OperatorType? Expected = expected;

    public static string GenerateMessage(OperatorType operation, OperatorType? expected)
    {
        if (expected is not null) return $"Expected operation {expected} not {operation}.";

        return $"Unexpected operation {operation}.";
    }
}