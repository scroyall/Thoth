using Thoth.Tokenization;

namespace Thoth.Parsing.Statements;

using Expressions;

public class ConditionalStatement
    : Statement
{
    public readonly Expression Condition;
    public readonly Statement Statement;

    public ConditionalStatement(Expression condition, Statement statement, SourceReference source)
        : base(source)
    {
        Condition = condition;
        Statement = statement;
    }

    protected override string ArgumentsToString()
        => $"Condition={Condition}, Statement={Statement}";
}