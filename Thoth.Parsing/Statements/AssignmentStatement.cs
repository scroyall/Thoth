using Thoth.Parsing.Expressions;
using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

public class AssignmentStatement
    : Statement
{
    public string Identifier { get; }
    public Expression Value { get; }

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
