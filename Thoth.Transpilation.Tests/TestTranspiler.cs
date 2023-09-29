using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;

namespace Thoth.Transpilation.Tests;

/// <summary>
/// Transpiler with support for fake expressions.
/// </summary>
public class TestTranspiler
    : Transpiler
{
    protected override BasicType TryGenerateExpression(Expression expression)
        => GenerateExpression(expression as dynamic);

    protected BasicType GenerateExpression(FakeExpression expression)
    {
        // Push NUL to fake the expression value.
        WriteCommentLine("fake expression");
        GeneratePush("0");

        return expression.Type.Resolved();
    }

    protected override bool TryGenerateStatement(Statement statement)
        => GenerateStatement(statement as dynamic);
    
    protected bool GenerateStatement(FakeStatement statement)
    {
        WriteCommentLine("fake statement");

        // Fake statements don't generate any statements to guarantee a return.
        return false;
    }
}