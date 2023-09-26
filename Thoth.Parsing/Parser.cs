namespace Thoth.Parsing;

using System.Reflection.Metadata.Ecma335;
using Expressions;
using Statements;
using Tokenization;
using Tokenization.Tokens;
using Utils;

public class UnexpectedTokenException(Token token)
    : Exception($"Unexpected token '{token}'.");

public class Parser
{
    private AtomStack<Token>? _tokens = null;

    private AtomStack<Token> Tokens => _tokens ?? throw new NullReferenceException();

    private IReadOnlyList<string>? _strings = null;

    private IReadOnlyList<string> Strings => _strings ?? throw new NullReferenceException();

    public ParsedProgram Parse(TokenizedProgram program)
    {
        // Reset cursor back to the first token.
        _tokens = new AtomStack<Token>(program.Tokens);
        _strings = program.Strings;

        // Create a list to hold parsed expressions.
        var statements = new List<Statement>();

        // Repeatedly parse statements until there are no tokens left.
        while (Tokens.Peek() is not null)
        {
            statements.Add(ParseStatement());
        }

        return new ParsedProgram(statements, Strings);
    }

    private Statement ParseStatement()
    {
        return Tokens.Peek() switch
        {
            KeywordToken keyword => ParseKeyword(keyword),
            SymbolToken symbol   => ParseSymbol(symbol),
            IdentifierToken      => ParseIdentifier(),
            TypeToken            => ParseDefinition(),
            { } token => throw new UnexpectedTokenException(token),
            null => throw new UnexpectedEndOfInputException()
        };
    }

    private Statement ParseKeyword(KeywordToken keyword)
    {
        return keyword.Type switch
        {
            KeywordType.Exit   => ParseExit(),
            KeywordType.Var    => ParseDefinition(),
            KeywordType.If     => ParseConditional(),
            KeywordType.While  => ParseWhile(),
            KeywordType.For    => ParseFor(),
            KeywordType.Assert => ParseAssert(),
            KeywordType.Print  => ParsePrint(),
            _ => throw new UnexpectedTokenException(keyword)
        };
    }

    private Statement ParseSymbol(SymbolToken symbol)
    {
        return symbol.Type switch
        {
            SymbolType.LeftBrace => ParseScope(),
            _ => throw new UnexpectedTokenException(symbol)
        };
    }

    private Statement ParseIdentifier()
    {
        // Check the next token after the identifier.
        return Tokens.Peek(1) switch
        {
            SymbolToken { Type: SymbolType.Equals } => ParseAssignment(),
            { } token => throw new UnexpectedTokenException(token),
            _ => throw new UnexpectedEndOfInputException()
        };
    }

    private AssignmentStatement ParseAssignment()
    {
        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeSymbol(SymbolType.Equals);

        var expression = ParseExpression();

        ConsumeSymbol(SymbolType.Semicolon);

        return new AssignmentStatement(identifier.Name, expression, identifier.Source);
    }

    private ScopeStatement ParseScope()
    {
        var leftBrace = ConsumeSymbol(SymbolType.LeftBrace);

        var statements = new List<Statement>();
        while (Tokens.Peek() is not SymbolToken { Type: SymbolType.RightBrace })
        {
            statements.Add(ParseStatement());
        }

        ConsumeSymbol(SymbolType.RightBrace);

        return new ScopeStatement(statements, leftBrace.Source);
    }

    private ExitStatement ParseExit()
    {
        var exit = ConsumeKeyword(KeywordType.Exit);
        ConsumeSymbol(SymbolType.LeftParenthesis);

        var expression = ParseExpression();

        ConsumeSymbol(SymbolType.RightParenthesis);
        ConsumeSymbol(SymbolType.Semicolon);

        return new ExitStatement(code: expression, source: exit.Source);
    }

    private DefinitionStatement ParseDefinition()
    {
        var type = ParseType(out var source);

        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeSymbol(SymbolType.Equals);

        var expression = ParseExpression();
        expression.Type.CheckMatches(type);

        ConsumeSymbol(SymbolType.Semicolon);

        return new DefinitionStatement(type ?? expression.Type, identifier.Name, expression, source);
    }

    private BasicType? ParseType(out SourceReference source)
    {
        switch (Tokens.Consume())
        {
            case TypeToken typeToken:
                source = typeToken.Source;
                return typeToken.Type;
            case KeywordToken { Type: KeywordType.Var } varToken:
                source = varToken.Source;
                return null;
            case { } token:
                throw new UnexpectedTokenException(token);
            default:
                throw new UnexpectedEndOfInputException();
        }
    }

    private ConditionalStatement ParseConditional()
    {
        var keyword = ConsumeKeyword(KeywordType.If);
        ConsumeSymbol(SymbolType.LeftParenthesis);

        var condition = ParseExpression();

        ConsumeSymbol(SymbolType.RightParenthesis);

        var statement = ParseStatement();

        return new ConditionalStatement(condition, statement, keyword.Source);
    }

    private WhileStatement ParseWhile()
    {
        var keyword = ConsumeKeyword(KeywordType.While);
        ConsumeSymbol(SymbolType.LeftParenthesis);

        var condition = ParseExpression();

        ConsumeSymbol(SymbolType.RightParenthesis);

        var statement = ParseStatement();

        return new WhileStatement(condition, statement, keyword.Source);
    }

    private EnumeratorStatement ParseFor()
    {
        var keyword = ConsumeKeyword(KeywordType.For);
        ConsumeSymbol(SymbolType.LeftParenthesis);

        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeKeyword(KeywordType.In);

        var range = ParseRange();

        ConsumeSymbol(SymbolType.RightParenthesis);

        var body = ParseStatement();

        return new EnumeratorStatement(identifier.Name, range, body, keyword.Source);
    }

    private AssertStatement ParseAssert()
    {
        var keyword = ConsumeKeyword(KeywordType.Assert);

        ConsumeSymbol(SymbolType.LeftParenthesis);

        var condition = ParseExpression();

        ConsumeSymbol(SymbolType.RightParenthesis);
        ConsumeSymbol(SymbolType.Semicolon);

        return new AssertStatement(condition, keyword.Source);
    }

    private PrintStatement ParsePrint()
    {
        var keyword = ConsumeKeyword(KeywordType.Print);

        ConsumeSymbol(SymbolType.LeftParenthesis);

        Expression expression;
        if (Tokens.Peek() is StringLiteralToken literal)
        {
            expression = new StringExpression(literal.Index);
            Tokens.Consume();
        }
        else
        {
            expression = ParseExpression();
        }

        ConsumeSymbol(SymbolType.RightParenthesis);
        ConsumeSymbol(SymbolType.Semicolon);

        return new PrintStatement(expression, keyword.Source);
    }

    private Expression ParseExpression(int minimumPrecedence = 0)
    {
        Expression left;

        switch (Tokens.Consume())
        {
            case IntegerLiteralToken integer:
                left = new IntegerExpression(integer.Value);
                break;
            case BooleanLiteralToken boolean:
                left = new BooleanLiteralExpression(boolean.Value);
                break;
            case IdentifierToken identifier:
                left = ParseVariableExpression(identifier);
                break;
            case SymbolToken { Type: SymbolType.LeftParenthesis }:
                left = ParseExpression();
                ConsumeSymbol(SymbolType.RightParenthesis);
                break;
            case SymbolToken { Type: SymbolType.Exclamation }:
                left = new UnaryOperationExpression(BasicType.Boolean, OperatorType.Not, ParseExpression());
                break;
            case { } token:
                throw new UnexpectedTokenException(token);
            default:
                throw new UnexpectedEndOfInputException();
        }

        // Check for a binary expression.
        while (PeekOperator(out var length) is { } operation &&
               IsBinaryOperator(operation, out var operatorPrecedence) &&
               operatorPrecedence >= minimumPrecedence)
        {
            ConsumeSymbols(length);

            // Parse the expression for the right hand side.
            var right = ParseExpression(operatorPrecedence);
            right.Type.CheckMatches(left.Type);

            // Binary expressions preferentially inherit their type from the left side, falling back to the right side.
            var type = left.Type ?? right.Type;

            if (operation.IsLogicalOperation())
            {
                // Both sides of logical operations must match the boolean type.
                left.Type.CheckMatches(BasicType.Boolean);
                right.Type.CheckMatches(BasicType.Boolean);

                // Logical operations are always of boolean type.
                type = BasicType.Boolean;
            }

            left = new BinaryOperationExpression(type, operation, left, right);
        }

        // Not a binary expression.
        return left;
    }

    private VariableExpression ParseVariableExpression(IdentifierToken identifier)
    {
        return new VariableExpression(identifier.Name);
    }

    private OperatorType? PeekOperator(out int length)
    {
        length = 1;

        if (Tokens.Peek() is not SymbolToken symbol) return null;

        switch (symbol.Type)
        {
            case SymbolType.Plus:
                return OperatorType.Add;
            case SymbolType.Star:
                return OperatorType.Multiply;
            case SymbolType.Minus:
                return OperatorType.Subtract;
            case SymbolType.ForwardSlash:
                return OperatorType.Divide;
            case SymbolType.Dot:
                if (Tokens.Peek(1) is not SymbolToken { Type: SymbolType.Dot }) return null;
                length = 2;
                return OperatorType.Range;
            case SymbolType.RightChevron:
                if (Tokens.Peek(1) is SymbolToken { Type: SymbolType.Equals })
                {
                    length = 2;
                    return OperatorType.GreaterThanOrEqual;
                }

                return OperatorType.GreaterThan;
            case SymbolType.LeftChevron:
                if (Tokens.Peek(1) is SymbolToken { Type: SymbolType.Equals })
                {
                    length = 2;
                    return OperatorType.LessThanOrEqual;
                }

                return OperatorType.LessThan;
            case SymbolType.Equals:
                if (Tokens.Peek(1) is SymbolToken { Type: SymbolType.Equals })
                {
                    length = 2;
                    return OperatorType.Equal;
                }

                break;
            case SymbolType.Exclamation:
                if (Tokens.Peek(1) is SymbolToken { Type: SymbolType.Equals })
                {
                    length = 2;
                    return OperatorType.NotEqual;
                }

                break;
            case SymbolType.Ampersand:
                if (Tokens.Peek(1) is SymbolToken { Type: SymbolType.Ampersand })
                {
                    length = 2;
                    return OperatorType.And;
                }

                break;
            case SymbolType.VerticalBar:
                if (Tokens.Peek(1) is SymbolToken { Type: SymbolType.VerticalBar })
                {
                    length = 2;
                    return OperatorType.Or;
                }

                break;
        }

        return null;
    }

    private RangeExpression ParseRange()
    {
        var start = ParseExpression();

        ConsumeSymbol(SymbolType.Dot);
        ConsumeSymbol(SymbolType.Dot);

        var end = ParseExpression();

        return new RangeExpression(start, end);
    }

    private KeywordToken ConsumeKeyword(KeywordType type)
    {
        var keyword = ConsumeToken<KeywordToken>();
        if (keyword.Type == type) return keyword;

        throw new UnexpectedTokenException(keyword);
    }

    private SymbolToken ConsumeSymbol(SymbolType type)
    {
        var symbol = ConsumeToken<SymbolToken>();
        if (symbol.Type == type) return symbol;

        throw new UnexpectedTokenException(symbol);
    }

    private void ConsumeSymbols(int count)
    {
        for (int i = 0; i < count; i++)
        {
            ConsumeToken<SymbolToken>();
        }
    }

    private TToken ConsumeToken<TToken>()
        where TToken : Token
    {
        if (Tokens.Consume<TToken>() is { } token) return token;
        if (Tokens.Peek() is { } other) throw new UnexpectedTokenException(other);

        throw new UnexpectedEndOfInputException();
    }

    private static bool IsBinaryOperator(OperatorType type, out int precedence)
    {
        precedence = GetBinaryOperatorPrecedence(type);
        return precedence > 0;
    }

    private static int GetBinaryOperatorPrecedence(OperatorType type)
    {
        switch (type)
        {
            case OperatorType.GreaterThan:
            case OperatorType.LessThan:
            case OperatorType.GreaterThanOrEqual:
            case OperatorType.LessThanOrEqual:
            case OperatorType.Equal:
            case OperatorType.NotEqual:
                return 1;
            case OperatorType.Add:
            case OperatorType.Subtract:
            case OperatorType.And:
            case OperatorType.Or:
                return 2;
            case OperatorType.Multiply:
            case OperatorType.Divide:
                return 3;
            default:
                return 0; // Other symbols are not binary operators and have no precedence.
        }
    }
}