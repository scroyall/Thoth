namespace Thoth.Parser;

using Expressions;
using Statements;
using Tokenizer;
using Tokenizer.Tokens;
using Utils;

public class UnexpectedTokenException
    : Exception
{
    public UnexpectedTokenException(Token token)
        : base($"Unexpected token '{token}'.")
    {
    }
}

public class Parser
{
    private readonly AtomStack<Token> _tokens;
    private readonly IReadOnlyList<string> _strings;

    public Parser(TokenizedProgram tokenized)
    {
        _tokens = new AtomStack<Token>(tokenized.Tokens);
        _strings = tokenized.Strings;
    }

    public ParsedProgram Parse()
    {
        // Reset cursor back to the first token.
        _tokens.Reset();

        // Create a list to hold parsed expressions.
        var statements = new List<Statement>();

        // Repeatedly parse statements until there are no tokens left.
        while (_tokens.Peek() is not null)
        {
            statements.Add(ParseStatement());
        }

        return new ParsedProgram(statements, _strings);
    }

    private Statement ParseStatement()
    {
        return _tokens.Peek() switch
        {
            KeywordToken keyword => ParseKeyword(keyword),
            SymbolToken symbol => ParseSymbol(symbol),
            IdentifierToken => ParseIdentifier(),
            { } token => throw new UnexpectedTokenException(token),
            null => throw new UnexpectedEndOfInputException()
        };
    }

    private Statement ParseKeyword(KeywordToken keyword)
    {
        return keyword.Type switch
        {
            KeywordType.Exit => ParseExit(),
            KeywordType.Let => ParseDefinition(),
            KeywordType.If => ParseConditional(),
            KeywordType.While => ParseWhile(),
            KeywordType.For => ParseFor(),
            KeywordType.Assert => ParseAssert(),
            KeywordType.Print => ParsePrint(),
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
        return _tokens.Peek(1) switch
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
        while (_tokens.Peek() is not SymbolToken { Type: SymbolType.RightBrace })
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
        var let = ConsumeKeyword(KeywordType.Let);
        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeSymbol(SymbolType.Equals);

        var expression = ParseExpression();

        ConsumeSymbol(SymbolType.Semicolon);

        return new DefinitionStatement(identifier.Name, expression, let.Source);
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
        if (_tokens.Peek() is StringLiteralToken literal)
        {
            expression = new StringExpression(literal.Index);
            _tokens.Consume();
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
        Expression expression;

        switch (_tokens.Consume())
        {
            case IntegerLiteralToken integer:
                expression = new IntegerExpression(integer.Value);
                break;
            case IdentifierToken identifier:
                expression = new VariableExpression(identifier.Name);
                break;
            case SymbolToken { Type: SymbolType.LeftParenthesis }:
                expression = ParseExpression();
                ConsumeSymbol(SymbolType.RightParenthesis);
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

            var right = ParseExpression(minimumPrecedence + 1);
            expression = new BinaryOperationExpression(operation, expression, right);
        }

        // Not a binary expression.
        return expression;
    }

    private OperatorType? PeekOperator(out int length)
    {
        length = 1;

        if (_tokens.Peek() is not SymbolToken symbol) return null;

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
                if (_tokens.Peek(1) is not SymbolToken { Type: SymbolType.Dot }) return null;
                length = 2;
                return OperatorType.Range;
            case SymbolType.RightChevron:
                if (_tokens.Peek(1) is SymbolToken { Type: SymbolType.Equals })
                {
                    length = 2;
                    return OperatorType.GreaterThanOrEqual;
                }

                return OperatorType.GreaterThan;
            case SymbolType.LeftChevron:
                if (_tokens.Peek(1) is SymbolToken { Type: SymbolType.Equals })
                {
                    length = 2;
                    return OperatorType.LessThanOrEqual;
                }

                return OperatorType.LessThan;
            case SymbolType.Equals:
                if (_tokens.Peek(1) is SymbolToken { Type: SymbolType.Equals })
                {
                    length = 2;
                    return OperatorType.Equal;
                }

                break;
            case SymbolType.Exclamation:
                if (_tokens.Peek(1) is SymbolToken { Type: SymbolType.Equals })
                {
                    length = 2;
                    return OperatorType.NotEqual;
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
        if (_tokens.Consume<TToken>() is { } token) return token;
        if (_tokens.Peek() is { } other) throw new UnexpectedTokenException(other);

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
            case OperatorType.Add:
            case OperatorType.Subtract:
                return 1;
            case OperatorType.Multiply:
            case OperatorType.Divide:
                return 2;
            case OperatorType.GreaterThan:
            case OperatorType.LessThan:
            case OperatorType.GreaterThanOrEqual:
            case OperatorType.LessThanOrEqual:
            case OperatorType.Equal:
            case OperatorType.NotEqual:
                return 3;
            default:
                return 0; // Other symbols are not binary operators and have no precedence.
        }
    }
}