using Thoth.Parsing.Expressions;
using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public class PrintStatement
    : Statement
{
    public readonly Expression Expression;

    public PrintStatement(Expression expression, SourceReference source)
        : base(source)
    {
        Expression = expression;
    }

    protected override string ArgumentsToString()
    {
        return Expression.ToString();
    }
}