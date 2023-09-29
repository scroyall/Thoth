using Thoth.Parsing.Expressions;
using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public record PrintStatement(Expression Value, SourceReference Source)
    : Statement(Source)
{
    protected override string ArgumentsToString()
    {
        return Value.ToString();
    }
}