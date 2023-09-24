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

public class UnexpectedOperationException(OperatorType type, string? message = null)
    : Exception($"Unexpected operation {type}{(message is not null ? "; " + message : string.Empty)}.");
