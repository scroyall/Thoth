namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public record WhileStatement(Expression Condition, Statement Body, SourceReference Source)
    : ConditionalStatement(Condition, Body, Source);