namespace Thoth.Parser.Statements;

using Tokenizer;

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
