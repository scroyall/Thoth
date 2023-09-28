using Thoth.Parsing.Statements;

namespace Thoth.Parsing;

public record ParsedProgram(
    IReadOnlyList<Statement> Statements,
    IReadOnlyList<string> Strings,
    IReadOnlyDictionary<string, DefinedFunction> Functions
);