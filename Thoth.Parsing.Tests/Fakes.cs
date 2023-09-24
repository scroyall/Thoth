using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public static class Fakes
{
    public static TokenizedProgram Program(string source)
        => new Tokenizer(source).Tokenize();
}