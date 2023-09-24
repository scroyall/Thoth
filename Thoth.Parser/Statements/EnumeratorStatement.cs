namespace Thoth.Parser.Statements;

using Expressions;
using Tokenizer;

public class EnumeratorStatement
    : Statement
{
    public readonly string Identifier;
    public readonly RangeExpression Range;
    public readonly Statement Body;

    public EnumeratorStatement(string identifier, RangeExpression range, Statement body, SourceReference source)
        : base(source)
    {
        Identifier = identifier;
        Range = range;
        Body = body;
    }
}
