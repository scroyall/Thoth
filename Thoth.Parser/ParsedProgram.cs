using Thoth.Parser.Statements;

namespace Thoth.Parser;

public readonly struct ParsedProgram
{
    public readonly IReadOnlyList<Statement> Statements;
    public readonly IReadOnlyList<string> Strings;

    public ParsedProgram(IReadOnlyList<Statement> statements, IReadOnlyList<string> strings)
    {
        Statements = statements;
        Strings = strings;
    }
}