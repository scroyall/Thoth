using Thoth.Parsing;
using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;

namespace Thoth.Transpilation;

public class UnexpectedStatementException(Statement statement)
    : Exception($"Unexpected statement '{statement}'.");

public class UnexpectedExpressionException(Expression expression)
    : Exception($"Unexpected expression {expression}.")
{
    public readonly Expression Expression = expression;
}

public class UndefinedVariableException(string identifier)
    : Exception($"Undefined variable '{identifier}'.");

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