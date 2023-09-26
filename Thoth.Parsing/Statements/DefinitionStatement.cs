namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public class DefinitionStatement(BasicType? type, string identifier, Expression value, SourceReference source)
    : AssignmentStatement(identifier, value, source)
{
    public BasicType? Type { get; } = type;
}