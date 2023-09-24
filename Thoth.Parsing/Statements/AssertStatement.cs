using Thoth.Parsing.Expressions;
using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

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