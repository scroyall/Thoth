using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing;

public record DefinedFunction(
    string Name,
    IReadOnlyList<NamedParameter> Parameters,
    // TODO Validate that the type is resolved.
    Type? ReturnType,
    Statement Body,
    SourceReference Source
);
