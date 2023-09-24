namespace Thoth.Parser.Statements;

using Expressions;
using Tokenizer;

public class AssignmentStatement
    : Statement
{
    public readonly string Identifier;
    public readonly Expression Value;

    public AssignmentStatement(string identifier, Expression value, SourceReference source)
        : base(source)
    {
        Identifier = identifier;
        Value = value;
    }

    protected override string ArgumentsToString()
    {
        return $"Identifier={Identifier}, Value={Value}";
    }
}
