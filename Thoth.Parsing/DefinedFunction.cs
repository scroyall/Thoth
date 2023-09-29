using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing;

public record DefinedFunction(
    string Name,
    BasicType? ReturnType,
    IReadOnlyList<Statement> Body,
    SourceReference Source
);