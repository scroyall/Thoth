namespace Thoth.Tokenization.Tokens;

public class BooleanLiteralToken(bool value, SourceReference source)
    : ValueToken<bool>(value, source);