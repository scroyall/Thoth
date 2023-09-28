namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public record ExitStatement(Expression Code, SourceReference Source)
    : Statement(Source)
{
    protected override string ArgumentsToString()
        => $"code {Code}";
}