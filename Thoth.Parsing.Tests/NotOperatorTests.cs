using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class NotOperatorTests
    : ParserTests
{
    [Test]
    public void NotOperator_Parses()
    {
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.SymbolToken(SymbolType.Exclamation);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(parsed.Statements, Has.Exactly(1).TypeOf<AssignmentStatement>());

        var assignment = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();
        Assert.That(assignment.Value, Is.TypeOf<UnaryOperationExpression>()
                                        .With.Property("Operation").EqualTo(OperatorType.Not));
    }
}
