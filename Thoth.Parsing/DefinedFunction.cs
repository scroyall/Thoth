using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing;

public record DefinedFunction(string Name, IReadOnlyList<Statement> Statements, SourceReference Source);