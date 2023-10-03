using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class FunctionCallTests
    : ParserTests
{
    [Test]
    public void FunctionCall_WithoutParameters_ParsesAsStatement()
    {
        var name = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<FunctionCallStatement>());
        var call = parsed.Statements[0] as FunctionCallStatement ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(name));
            Assert.That(call.Parameters, Is.Empty);
        });
    }

    [Test]
    public void FunctionCall_WithoutParameters_ParsesAsExpression()
    {
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        var name = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());
        var assignment = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        Assert.That(assignment.Value, Is.TypeOf<FunctionCallExpression>());
        var call = assignment.Value as FunctionCallExpression ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(name));
            Assert.That(call.Parameters, Is.Empty);
        });

    }

    [Test]
    public void FunctionCall_WithSingleParameter_ParsesAsStatement()
    {
        var functionName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        var parameterName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<FunctionCallStatement>());
        var call = parsed.Statements[0] as FunctionCallStatement ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(functionName));
            Assert.That(call.Parameters, Is.EquivalentTo(new[]
            {
                new VariableExpression(parameterName)
            }));
        });
    }

    [Test]
    public void FunctionCall_WithSingleParameter_ParsesAsExpression()
    {

        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        var functionName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        var parameterName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());
        var assignment = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        Assert.That(assignment.Value, Is.TypeOf<FunctionCallExpression>());
        var call = assignment.Value as FunctionCallExpression ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(functionName));
            Assert.That(call.Parameters, Is.EquivalentTo(new[]
            {
                new VariableExpression(parameterName)
            }));
        });
    }

    [Test]
    public void FunctionCall_WithMultipleParameters_ParsesAsStatement()
    {
        var functionName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        var firstName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.Comma);
        var secondName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<FunctionCallStatement>());
        var call = parsed.Statements[0] as FunctionCallStatement ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(functionName));
            Assert.That(call.Parameters, Is.EquivalentTo(new[]
            {
                new VariableExpression(firstName),
                new VariableExpression(secondName)
            }));
        });
    }

    [Test]
    public void FunctionCall_WithMultipleParameters_ParsesAsExpression()
    {
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        var functionName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftParenthesis);
        var firstName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.Comma);
        var secondName = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.RightParenthesis);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());
        var assignment = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        Assert.That(assignment.Value, Is.TypeOf<FunctionCallExpression>());
        var call = assignment.Value as FunctionCallExpression ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(functionName));
            Assert.That(call.Parameters, Is.EquivalentTo(new[]
            {
                new VariableExpression(firstName),
                new VariableExpression(secondName)
            }));
        });
    }
}
