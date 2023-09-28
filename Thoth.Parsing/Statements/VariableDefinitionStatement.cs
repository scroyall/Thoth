namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public record VariableDefinitionStatement(BasicType? Type, string Identifier, Expression Value, SourceReference Source)
    : AssignmentStatement(Identifier, Value, Source);