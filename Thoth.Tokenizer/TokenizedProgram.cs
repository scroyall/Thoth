namespace Thoth.Tokenizer;

public readonly struct TokenizedProgram
{
    public readonly IReadOnlyList<Token> Tokens;
    public readonly IReadOnlyList<string> Strings;

    public TokenizedProgram(IReadOnlyList<Token> tokens, IReadOnlyList<string> strings)
    {
        Tokens = tokens;
        Strings = strings;
    }
}