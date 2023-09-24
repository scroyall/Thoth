namespace Thoth.Parsing.Statements;

using Expressions;
using Tokenization;

public class ExitStatement
    : Statement
{
    public readonly Expression Code;

    public ExitStatement(Expression code, SourceReference source)
        : base(source)
    {
        Code = code;
    }

    protected override string ArgumentsToString()
    {
        return $"Code={Code}";
    }
}
