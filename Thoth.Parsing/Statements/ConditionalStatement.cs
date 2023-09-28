using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

using Expressions;

public record ConditionalStatement(Expression Condition, Statement Body, SourceReference Source)
    : Statement(Source)
{
    protected override string ArgumentsToString()
        => $"if {Condition} then {Body}";
}