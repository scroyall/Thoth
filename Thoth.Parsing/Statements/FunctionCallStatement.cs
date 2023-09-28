using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public record FunctionCallStatement(string Name, SourceReference Source)
    : Statement(Source);