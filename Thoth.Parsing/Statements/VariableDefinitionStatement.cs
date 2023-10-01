namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public record VariableDefinitionStatement(IType Type, string Identifier, Expression Value, SourceReference Source)
    : AssignmentStatement(Identifier, Value, Source);