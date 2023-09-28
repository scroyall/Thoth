using Thoth.Parsing.Expressions;
using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public record AssertStatement(Expression Condition, SourceReference Source)
    : Statement(Source);