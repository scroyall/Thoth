namespace Thoth.Parser.Statements;

using Expressions;
using Tokenizer;

public class WhileStatement
    : ConditionalStatement
{
    public WhileStatement(Expression condition, Statement statement, SourceReference source)
        : base(condition, statement, source)
    { }
}
