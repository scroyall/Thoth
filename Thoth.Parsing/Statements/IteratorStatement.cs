namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public record IteratorStatement(string Identifier, Expression Iterable, Statement Body, SourceReference Source)
    : Statement(Source);
