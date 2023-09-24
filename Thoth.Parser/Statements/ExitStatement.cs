namespace Thoth.Parser.Statements;

using Expressions;
using Tokenizer;

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
