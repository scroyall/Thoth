namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public class DefinitionStatement
    : AssignmentStatement
{
    public DefinitionStatement(string identifier, Expression value, SourceReference source)
        : base(identifier, value, source)
    { }
}
