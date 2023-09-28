using Thoth.Parsing.Expressions;
using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public record AssignmentStatement(string Identifier, Expression Value, SourceReference Source)
    : Statement(Source)
{
    protected override string ArgumentsToString()
        => $"{Identifier} = {Value}";
}