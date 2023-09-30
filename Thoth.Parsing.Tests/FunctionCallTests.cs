using System.Diagnostics;
using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class FunctionCallTests
    : ParserTests
{
    [Test]
    public void FunctionCallWithoutParameters_Parses_AsStatement()
    {
        var name = Fakes.IdentifierName;

        var program = Parse(
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<FunctionCallStatement>());
        var call = program.Statements[0] as FunctionCallStatement ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(name));
            Assert.That(call.Parameters, Is.Empty);
        });
    }

    [Test]
    public void FunctionCallWithoutParameters_Parses_AsExpression()
    {
        var name = Fakes.IdentifierName;

        var program = Parse(
            Fakes.Keyword(KeywordType.Print),
            Fakes.Symbol(SymbolType.LeftParenthesis),

            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Symbol(SymbolType.RightParenthesis),

            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<PrintStatement>());
        var print = program.Statements[0] as PrintStatement ?? throw new NullReferenceException();

        Assert.That(print.Value, Is.TypeOf<FunctionCallExpression>());
        var call = print.Value as FunctionCallExpression ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(name));
            Assert.That(call.Parameters, Is.Empty);
        });

    }

    [Test]
    public void FunctionCallWithSingleParameter_Parses_AsStatement()
    {
        var variableName = Fakes.IdentifierName;

        var name = Fakes.IdentifierName;
        var program = Parse(
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Identifier(variableName),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<FunctionCallStatement>());
        var call = program.Statements[0] as FunctionCallStatement ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(name));
            Assert.That(call.Parameters, Is.EquivalentTo(new[]
            {
                new VariableExpression(variableName)
            }));
        });
    }

    [Test]
    public void FunctionCallWithSingleParameter_Parses_AsExpression()
    {
        var variableName = Fakes.IdentifierName;

        var name = Fakes.IdentifierName;
        var program = Parse(
            Fakes.Keyword(KeywordType.Print),
            Fakes.Symbol(SymbolType.LeftParenthesis),

            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Identifier(variableName),
            Fakes.Symbol(SymbolType.RightParenthesis),

            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<PrintStatement>());
        var print = program.Statements[0] as PrintStatement ?? throw new NullReferenceException();

        Assert.That(print.Value, Is.TypeOf<FunctionCallExpression>());
        var call = print.Value as FunctionCallExpression ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(name));
            Assert.That(call.Parameters, Is.EquivalentTo(new[]
            {
                new VariableExpression(variableName)
            }));
        });
    }

    [Test]
    public void FunctionCallWithMultipleParameters_Parses_AsStatement()
    {
        var firstName = Fakes.IdentifierName;
        var secondName = Fakes.IdentifierName;

        var name = Fakes.IdentifierName;
        var program = Parse(
            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Identifier(firstName),
            Fakes.Symbol(SymbolType.Comma),
            Fakes.Identifier(secondName),
            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<FunctionCallStatement>());
        var call = program.Statements[0] as FunctionCallStatement ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(name));
            Assert.That(call.Parameters, Is.EquivalentTo(new[]
            {
                new VariableExpression(firstName),
                new VariableExpression(secondName)
            }));
        });
    }

    [Test]
    public void FunctionCallWithMultipleParameters_Parses_AsExpression()
    {
        var firstName = Fakes.IdentifierName;
        var secondName = Fakes.IdentifierName;

        var name = Fakes.IdentifierName;
        var program = Parse(
            Fakes.Keyword(KeywordType.Print),
            Fakes.Symbol(SymbolType.LeftParenthesis),

            Fakes.Identifier(name),
            Fakes.Symbol(SymbolType.LeftParenthesis),
            Fakes.Identifier(firstName),
            Fakes.Symbol(SymbolType.Comma),
            Fakes.Identifier(secondName),
            Fakes.Symbol(SymbolType.RightParenthesis),

            Fakes.Symbol(SymbolType.RightParenthesis),
            Fakes.Symbol(SymbolType.Semicolon)
        );

        Assert.That(program.Statements, Has.Count.EqualTo(1).And.One.TypeOf<PrintStatement>());
        var print = program.Statements[0] as PrintStatement ?? throw new NullReferenceException();

        Assert.That(print.Value, Is.TypeOf<FunctionCallExpression>());
        var call = print.Value as FunctionCallExpression ?? throw new NullReferenceException();

        Assert.Multiple(() =>
        {
            Assert.That(call.Name, Is.EqualTo(name));
            Assert.That(call.Parameters, Is.EquivalentTo(new[]
            {
                new VariableExpression(firstName),
                new VariableExpression(secondName)
            }));
        });
    }
}