namespace Thoth.Parsing;

using Expressions;
using Statements;
using Tokenization;
using Tokenization.Tokens;
using Utils;

public class UnexpectedTokenException(Token token)
    : Exception($"Unexpected token '{token}' at {token.Source}.");

public class MultiplyDefinedFunctionException(string name)
    : Exception($"Function '{name}' is already defined.");

public class MultiplyDefinedClassException(string name)
    : Exception($"Class '{name}' is already defined.");

public class Parser
{
    private AtomStack<Token>? _tokens = null;

    private AtomStack<Token> Tokens => _tokens ?? throw new NullReferenceException();

    private IReadOnlyList<string>? _strings = null;

    private IReadOnlyList<string> Strings => _strings ?? throw new NullReferenceException();

    private List<Statement> Statements { get; } = [];

    private Dictionary<string, DefinedFunction> Functions { get; } = [];

    private Dictionary<string, DefinedClass> Classes { get; } = [];

    public ParsedProgram Parse(TokenizedProgram program)
    {
        // Reset cursor back to the first token.
        _tokens = new AtomStack<Token>(program.Tokens);
        _strings = program.Strings;

        Statements.Clear();
        Functions.Clear();

        // Repeatedly parse statements until there are no tokens left.
        while (Tokens.Peek() is not null)
        {
            Statements.Add(ParseStatement());
        }

        return new ParsedProgram(Statements, Strings, Functions, Classes);
    }

    private Statement ParseStatement()
    {
        return Tokens.Peek() switch
        {
            KeywordToken keyword => ParseKeyword(keyword),
            SymbolToken symbol   => ParseSymbol(symbol),
            IdentifierToken      => ParseIdentifier(),
            BuiltinTypeToken            => ParseVariableDefinition(),
            { } token => throw new UnexpectedTokenException(token),
            null => throw new UnexpectedEndOfInputException()
        };
    }

    private Statement ParseKeyword(KeywordToken keyword)
    {
        return keyword.Type switch
        {
            KeywordType.Exit     => ParseExit(),
            KeywordType.If       => ParseConditional(),
            KeywordType.While    => ParseWhile(),
            KeywordType.For      => ParseIterator(),
            KeywordType.Assert   => ParseAssert(),
            KeywordType.Print    => ParsePrint(),
            KeywordType.Function => ParseFunctionDefinition(),
            KeywordType.Return   => ParseReturn(),
            KeywordType.Class    => ParseClass(),
            _ => throw new UnexpectedTokenException(keyword)
        };
    }

    private ClassDefinitionStatement ParseClass()
    {
        var keyword = ConsumeKeyword(KeywordType.Class);
        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeSymbol(SymbolType.LeftBrace);
        ConsumeSymbol(SymbolType.RightBrace);

        DefineClass(identifier.Name);
        return new(identifier.Name, keyword.Source);
    }

    private ReturnStatement ParseReturn()
    {
        var keyword = ConsumeKeyword(KeywordType.Return);

        Expression? value = null;
        if (Tokens.Peek() is not SymbolToken { Type: SymbolType.Semicolon })
        {
            value = ParseExpression();
        }

        ConsumeSymbol(SymbolType.Semicolon);

        return new ReturnStatement(value, keyword.Source);
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
            SymbolToken { Type: SymbolType.LeftParenthesis } => ParseFunctionCallStatement(),
            { } token => throw new UnexpectedTokenException(token),
            _ => throw new UnexpectedEndOfInputException()
        };
    }

    private FunctionCallStatement ParseFunctionCallStatement()
    {
        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeSymbol(SymbolType.LeftParenthesis);

        var parameters = ParseExpressionsUntil(SymbolType.RightParenthesis).ToList();

        ConsumeSymbol(SymbolType.RightParenthesis);
        ConsumeSymbol(SymbolType.Semicolon);

        return new FunctionCallStatement(identifier.Name, parameters, identifier.Source);
    }

    /// <summary>
    /// Parse comma separated expressions until a terminator is reached.
    /// The terminator is not consumed.
    /// </summary>
    private IEnumerable<Expression> ParseExpressionsUntil(SymbolType terminator)
    {
        // No parameters at all if the first next token is the terminator.
        if (IsSymbolNext(terminator)) yield break;

        // Repeatedly parse as many parameters as possible.
        while (true)
        {
            yield return ParseExpression();

            if (IsSymbolNext(terminator)) yield break;

            ConsumeSymbol(SymbolType.Comma);
        }
    }

    private AssignmentStatement ParseAssignment()
    {
        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeSymbol(SymbolType.Equals);

        var expression = ParseExpression();

        ConsumeSymbol(SymbolType.Semicolon);

        return new AssignmentStatement(identifier.Name, expression, identifier.Source);
    }

    private List<Statement> ParseBlock(out SourceReference source)
    {
        source = ConsumeSymbol(SymbolType.LeftBrace).Source;

        var statements = new List<Statement>();
        while (Tokens.Peek() is not SymbolToken { Type: SymbolType.RightBrace })
        {
            statements.Add(ParseStatement());
        }

        ConsumeSymbol(SymbolType.RightBrace);

        return statements;
    }

    private ScopeStatement ParseScope()
    {
        var statements = ParseBlock(out var source);

        return new ScopeStatement(statements, source);
    }

    private ExitStatement ParseExit()
    {
        var exit = ConsumeKeyword(KeywordType.Exit);
        ConsumeSymbol(SymbolType.LeftParenthesis);

        var expression = ParseExpression();

        ConsumeSymbol(SymbolType.RightParenthesis);
        ConsumeSymbol(SymbolType.Semicolon);

        return new ExitStatement(expression, exit.Source);
    }

    private VariableDefinitionStatement ParseVariableDefinition()
    {
        var type = ParseType(out var source);

        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeSymbol(SymbolType.Equals);

        var expression = ParseExpression();

        ConsumeSymbol(SymbolType.Semicolon);

        return new VariableDefinitionStatement(type, identifier.Name, expression, source);
    }

    private FunctionDefinitionStatement ParseFunctionDefinition()
    {
        var source = ConsumeKeyword(KeywordType.Function).Source;
        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeSymbol(SymbolType.LeftParenthesis);

        var parameters = ParseNamedParameters().ToList();

        ConsumeSymbol(SymbolType.RightParenthesis);

        Type? type = null;
        if (TryConsumeSymbol(SymbolType.Minus))
        {
            ConsumeSymbol(SymbolType.RightChevron);

            type = ParseType(out _);
        }

        var body = ParseStatement();

        DefineFunction(identifier.Name, parameters, type, body, identifier.Source);
        return new FunctionDefinitionStatement(identifier.Name, source);
    }

    private IEnumerable<NamedParameter> ParseNamedParameters()
    {
        // Next token must be a type to parse any named parameters.
        if (Tokens.Peek() is not BuiltinTypeToken) yield break;

        // Keep parsing named parameters for as long as possible.
        while (true)
        {
            yield return ParseNamedParameter();

            // Finish parsing named parameters unless there's a comma.
            if (!TryConsumeSymbol(SymbolType.Comma)) yield break;
        }
    }

    private NamedParameter ParseNamedParameter()
    {
        var type = ParseType(out _);
        var name = ConsumeToken<IdentifierToken>().Name;

        return new(type, name);
    }

    private Type ParseType(out SourceReference source)
    {
        var outer = ConsumeToken<BuiltinTypeToken>();
        source = outer.Source;

        Type[] parameters = [];
        if (TryConsumeSymbol(SymbolType.LeftChevron))
        {
            parameters = [ParseType(out _)];

            ConsumeSymbol(SymbolType.RightChevron);
        }

        return new(outer.Type, parameters);
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

    private IteratorStatement ParseIterator()
    {
        var keyword = ConsumeKeyword(KeywordType.For);
        ConsumeSymbol(SymbolType.LeftParenthesis);

        var identifier = ConsumeToken<IdentifierToken>();

        ConsumeKeyword(KeywordType.In);

        var iterable = ParseExpression();

        ConsumeSymbol(SymbolType.RightParenthesis);

        var body = ParseStatement();

        return new IteratorStatement(identifier.Name, iterable, body, keyword.Source);
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

        var expression = ParseExpression();

        ConsumeSymbol(SymbolType.RightParenthesis);
        ConsumeSymbol(SymbolType.Semicolon);

        return new PrintStatement(expression, keyword.Source);
    }

    private Expression ParseExpression(int minimumPrecedence = 0)
    {
        Expression left;

        switch (Tokens.Consume())
        {
            case IntegerLiteralToken integer: // Integer Literal
                left = new IntegerExpression(integer.Value);
                break;
            case BooleanLiteralToken boolean: // Boolean Literal
                left = new BooleanLiteralExpression(boolean.Value);
                break;
            case StringLiteralToken string_: // String Literal
                left = new StringExpression(string_.Index);
                break;
            case IdentifierToken identifier: // Variable or Function Call
                if (Tokens.Peek() is SymbolToken { Type: SymbolType.LeftParenthesis }) // Function Call
                {
                    left = ParseFunctionCallExpression(identifier);
                    break;
                }

                left = ParseVariableExpression(identifier);
                break;
            case SymbolToken { Type: SymbolType.LeftParenthesis }: // Bracketed Expression
                left = ParseExpression();
                ConsumeSymbol(SymbolType.RightParenthesis);
                break;
            case SymbolToken { Type: SymbolType.Exclamation }: // Not Operation
                left = new UnaryOperationExpression(OperatorType.Not, ParseExpression());
                break;
            case BuiltinTypeToken { Type: BuiltinType.List } token:
                left = ParseListLiteralExpression(token);
                break;
            case { } token:
                throw new UnexpectedTokenException(token);
            default:
                throw new UnexpectedEndOfInputException();
        }

        // Check for an index expression.
        while (TryConsumeSymbol(SymbolType.LeftSquareBracket))
        {
            var index = ParseExpression();

            ConsumeSymbol(SymbolType.RightSquareBracket);

            left = new IndexExpression(left, index);
        }

        // Check for a binary expression.
        while (PeekOperator(out var length) is { } operation &&
               IsBinaryOperator(operation, out var operatorPrecedence) &&
               operatorPrecedence >= minimumPrecedence)
        {
            ConsumeSymbols(length);

            // Parse the expression for the right hand side.
            var right = ParseExpression(operatorPrecedence);

            left = new BinaryOperationExpression(operation, left, right);
        }

        // Not a binary expression.
        return left;
    }

    private ListLiteralExpression ParseListLiteralExpression(BuiltinTypeToken token)
    {
        if (token.Type != BuiltinType.List) throw new UnexpectedTokenException(token);

        ConsumeSymbol(SymbolType.LeftChevron);

        var memberType = ParseType(out _);

        ConsumeSymbol(SymbolType.RightChevron);
        ConsumeSymbol(SymbolType.LeftSquareBracket);

        var members = ParseExpressionsUntil(SymbolType.RightSquareBracket).ToList();

        ConsumeSymbol(SymbolType.RightSquareBracket);

        return new(memberType, members);
    }

    private FunctionCallExpression ParseFunctionCallExpression(IdentifierToken identifier)
    {
        ConsumeSymbol(SymbolType.LeftParenthesis);

        var parameters = ParseExpressionsUntil(SymbolType.RightParenthesis).ToList();

        ConsumeSymbol(SymbolType.RightParenthesis);

        return new FunctionCallExpression(identifier.Name, parameters);
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

    private KeywordToken ConsumeKeyword(KeywordType type)
    {
        var keyword = ConsumeToken<KeywordToken>();
        if (keyword.Type == type) return keyword;

        throw new UnexpectedTokenException(keyword);
    }

    private bool TryConsumeSymbol(SymbolType type)
    {
        if (Tokens.Peek() is SymbolToken symbol && symbol.Type == type)
        {
            ConsumeSymbol(type);
            return true;
        }

        return false;
    }

    private bool IsSymbolNext(SymbolType type)
        => Tokens.Peek() is SymbolToken symbol && symbol.Type == type;

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
            case OperatorType.Range:
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

    private void DefineFunction(string name, IReadOnlyList<NamedParameter> parameters, Type? returnType, Statement body, SourceReference source)
    {
        if (Functions.ContainsKey(name)) throw new MultiplyDefinedFunctionException(name);

        Functions[name] = new DefinedFunction(name, parameters, returnType, body, source);
    }

    private void DefineClass(string name)
    {
        if (Classes.ContainsKey(name)) throw new MultiplyDefinedClassException(name);

        Classes[name] = new DefinedClass(name, Type.Object);
    }
}
