namespace Thoth.Transpiler;

using Parser.Expressions;
using Parser.Statements;

public class UnexpectedStatementException
    : Exception
{
    public UnexpectedStatementException(Statement statement)
        : base($"Unexpected statement '{statement.GetType().Name}'.")
    { }
}

public class UnexpectedExpressionException
    : Exception
{
    public UnexpectedExpressionException(Expression expression)
        : base($"Unexpected expression '{expression.GetType().Name}'.")
    { }
}

public class UndefinedVariableException
    : Exception
{
    public UndefinedVariableException(string identifier)
        : base($"Undefined variable '{identifier}'.")
    { }
}

public class MultiplyDefinedVariableException
    : Exception
{
    public MultiplyDefinedVariableException(string identifier)
        : base($"Variable '{identifier}' is already defined.")
    { }
}
