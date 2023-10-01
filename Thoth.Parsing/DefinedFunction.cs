using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing;

public record DefinedFunction(
    string Name,
    IReadOnlyList<NamedParameter> Parameters,
    IResolvedType? ReturnType,
    Statement Body,
    SourceReference Source
);