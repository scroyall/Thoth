using Thoth.Parsing.Expressions;
using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public record FunctionCallStatement(string Name, IReadOnlyList<Expression> Parameters, SourceReference Source)
    : Statement(Source)
    , IFunctionCall;
