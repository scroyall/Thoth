using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public record ClassDefinitionStatement(string Name, SourceReference Source)
    : Statement(Source);
