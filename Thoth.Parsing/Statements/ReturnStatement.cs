using Thoth.Parsing.Expressions;
using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public record ReturnStatement(Expression? Value, SourceReference Source)
    : Statement(Source);