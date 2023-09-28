namespace Thoth.Parsing.Statements;

using Tokenization;

public record ScopeStatement(IReadOnlyList<Statement> Statements, SourceReference Source)
    : Statement(Source)
{
    protected override string ArgumentsToString()
        => string.Join(" ", Statements);
}