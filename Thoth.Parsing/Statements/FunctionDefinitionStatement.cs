using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public record FunctionDefinitionStatement(string Name, SourceReference Source)
    : Statement(Source);