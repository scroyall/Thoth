namespace Thoth.Parser.Statements;

using Expressions;
using Tokenizer;

public class DefinitionStatement
    : AssignmentStatement
{
    public DefinitionStatement(string identifier, Expression value, SourceReference source)
        : base(identifier, value, source)
    { }
}
