using Thoth.Parser.Expressions;
using Thoth.Tokenizer;

namespace Thoth.Parser.Statements;

public class AssertStatement
    : Statement
{
    public readonly Expression Condition;

    public AssertStatement(Expression condition, SourceReference source)
        : base(source)
    {
        Condition = condition;
    }
}