using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tests;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class BinaryLogicalOperatorTests
    : ParserTests
{
    [Test]
    public void BinaryLogicalOperator_WithBooleanOperands_Parses(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        var tokens = new List<Token> {
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
        };
        Fakes.Literal(ref tokens, BasicType.Boolean);
        Fakes.Operation(ref tokens, operation);
        Fakes.Literal(ref tokens, BasicType.Boolean);
        tokens.Add(Fakes.Symbol(SymbolType.Semicolon));

        var program = Parse(Fakes.Program(tokens));

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(BasicType.Boolean),
                                           "Expected definition statement of boolean type.");

        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();
        Assert.That(definition.Value, Is.TypeOf<BinaryOperationExpression>()
                                        .With.Property("Operation").EqualTo(operation),
                                        $"Expected binary expression for operation {operation}.");
    }

    [Test]
    public void BinaryLogicalOperator_WithUnresolvedOperands_Parses(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        var tokens = new List<Token> {
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Identifier(),
        };
        Fakes.Operation(ref tokens, operation);
        tokens.Add(Fakes.Identifier());
        tokens.Add(Fakes.Symbol(SymbolType.Semicolon));

        var program = Parse(Fakes.Program(tokens));

        Assert.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
        Assert.That(program.Statements, Has.Exactly(1).TypeOf<VariableDefinitionStatement>()
                                           .With.Property("Type").EqualTo(BasicType.Boolean).Or.Null,
                                           "Expected definition statement of boolean type.");

        var definition = program.Statements[0] as VariableDefinitionStatement ?? throw new NullReferenceException();
        Assert.That(definition.Value, Is.TypeOf<BinaryOperationExpression>()
                                        .With.Property("Operation").EqualTo(operation),
                                        $"Expected binary expression for operation {operation}.");
    }

    [Test]
    public void BinaryLogicalOperator_WithNonBooleanResolvedOperand_ThrowsException(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation,
        [ValueSource(typeof(ResolvedBasicType), nameof(ResolvedBasicType.Values))] ResolvedBasicType leftType,
        [ValueSource(typeof(ResolvedBasicType), nameof(ResolvedBasicType.Values))] ResolvedBasicType rightType)
    {
        if (leftType.Matches(BasicType.Boolean) && rightType.Matches(BasicType.Boolean)) Assert.Ignore("Both operand types are boolean.");

        var tokens = new List<Token> {
            Fakes.Type(BasicType.Unresolved),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals)
        };
        Fakes.Literal(ref tokens, leftType);
        Fakes.Operation(ref tokens, operation);
        Fakes.Literal(ref tokens, rightType);
        tokens.Add(Fakes.Symbol(SymbolType.Semicolon));

        Assert.Throws<MismatchedTypeException>(() => Parse(tokens));
    }
}
