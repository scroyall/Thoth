namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public record EnumeratorStatement(string Identifier, RangeExpression Range, Statement Body, SourceReference Source)
    : Statement(Source);