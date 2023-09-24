namespace Thoth.Parsing.Statements;

using Tokenization;

public class ScopeStatement
    : Statement
{
    public readonly IReadOnlyList<Statement> Statements;

    public ScopeStatement(IReadOnlyList<Statement> statements, SourceReference source)
        : base(source)
    {
        Statements = statements;
    }

    protected override string ArgumentsToString() => string.Join(" ", Statements);
}
