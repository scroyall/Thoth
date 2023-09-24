namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public class WhileStatement
    : ConditionalStatement
{
    public WhileStatement(Expression condition, Statement statement, SourceReference source)
        : base(condition, statement, source)
    { }
}
