using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tests;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class BinaryLogicalOperatorTests
    : ParserTests
{
    [Test]
    public void BinaryLogicalOperator_WithLiteralOperands_Parses(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        Program.BuiltinTypeToken(BuiltinType.Bool);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.LiteralTokens(Type.Boolean);
        Program.OperatorTokens(operation);
        Program.LiteralTokens(Type.Boolean);
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(parsed.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(Type.Boolean),
                                           "Expected definition statement of boolean type.");

        var definition = parsed.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();
        Assert.That(definition.Value, Is.TypeOf<BinaryOperationExpression>()
                                        .With.Property("Operation").EqualTo(operation),
                                        $"Expected binary expression for operation {operation}.");
    }

    [Test]
    public void BinaryLogicalOperator_WithVariableOperands_Parses(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        Program.BuiltinTypeToken(BuiltinType.Bool);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Equals);
        Program.IdentifierToken();
        Program.OperatorTokens(operation);
        Program.IdentifierToken();
        Program.SymbolToken(SymbolType.Semicolon);

        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(parsed.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(Type.Boolean).Or.Null,
                                           "Expected definition statement of boolean type.");

        var definition = parsed.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();
        Assert.That(definition.Value, Is.TypeOf<BinaryOperationExpression>()
                                        .With.Property("Operation").EqualTo(operation),
                                        $"Expected binary expression for operation {operation}.");
    }
}
