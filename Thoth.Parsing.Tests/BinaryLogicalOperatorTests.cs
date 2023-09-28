using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;
using Thoth.Tests;
using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class LogicalOperatorTests
    : ParserTests
{
    [Test]
    public void BinaryLogicalOperator_Parses_WhenOperandsAreBoolean(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        var tokens = new List<Token> {
            Fakes.Keyword(KeywordType.Var),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.BooleanLiteral,
        };
        Fakes.Operation(ref tokens, operation);
        tokens.Add(Fakes.BooleanLiteral);
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
    public void BinaryLogicalOperator_Parses_WhenOperandTypesAreUnresolved(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation)
    {
        var tokens = new List<Token> {
            Fakes.Keyword(KeywordType.Var),
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
    public void BinaryLogicalOperator_ThrowsException_WhenOperandTypesAreNotBoolean(
        [ValueSource(typeof(OperatorValueSources), nameof(OperatorValueSources.BinaryLogical))] OperatorType operation,
        [Values] BasicType leftType,
        [Values] BasicType rightType)
    {
        if (leftType == BasicType.Boolean && rightType == BasicType.Boolean) Assert.Ignore("Both operand types are boolean.");

        var tokens = new List<Token> {
            Fakes.Keyword(KeywordType.Var),
            Fakes.Identifier(),
            Fakes.Symbol(SymbolType.Equals),
            Fakes.Literal(leftType),
        };
        Fakes.Operation(ref tokens, operation);
        tokens.Add(Fakes.Literal(rightType));
        tokens.Add(Fakes.Symbol(SymbolType.Semicolon));

        Assert.Throws<MismatchedTypeException>(() => Parse(tokens));
    }

    // [Test]
    // public void NotExpression_ParsesWhenValueTypeIsUnresolved()
    // {
    //     var program = Parse(
    //         Fakes.Keyword(KeywordType.Var),
    //         Fakes.Identifier(),
    //         Fakes.Symbol(SymbolType.Equals),
    //         Fakes.Symbol(SymbolType.Exclamation),
    //         Fakes.Identifier(),
    //         Fakes.Symbol(SymbolType.Semicolon)
    //     );

    //     Assume.That(program.Statements, Has.Count.EqualTo(1), "Expected exactly one statement.");
    //     Assume.That(program.Statements, Has.Exactly(1).TypeOf<DefinitionStatement>()
    //                                        .With.Property("Type").EqualTo(BasicType.Boolean),
    //                                        "Expected definition statement of boolean type.");

    //     var definition = program.Statements[0] as DefinitionStatement ?? throw new NullReferenceException();
    //     Assert.That(definition.Value, Is.TypeOf<UnaryOperationExpression>()
    //                                     .With.Property("Operation").EqualTo(OperatorType.Not),
    //                                     "Expected unary expression for not operation.");
    // }

    // [Test]
    // public void NotExpression_ThrowInvalidTypeException_WhenValueIsNotBoolean([Values] BasicType valueType)
    // {
    //     if (valueType == BasicType.Boolean) Assert.Ignore("Value type is boolean.");
    //     if (valueType == BasicType.String)  Assert.Ignore("String literals cannot be parsed as expressions yet.");

    //     Assert.Throws<MismatchedTypeException>(() => Parse(
    //         Fakes.Keyword(KeywordType.Var),
    //         Fakes.Identifier(),
    //         Fakes.Symbol(SymbolType.Equals),
    //         Fakes.Symbol(SymbolType.Exclamation),
    //         Fakes.Literal(valueType),
    //         Fakes.Symbol(SymbolType.Semicolon)
    //     ));
    // }
}