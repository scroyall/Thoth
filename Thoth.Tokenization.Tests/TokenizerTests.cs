namespace Thoth.Tokenization.Tests;

[Parallelizable]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class TokenizerTests
{
    protected Tokenizer Tokenizer { get; } = new();

    protected TokenizedProgram Tokenize(string input)
        => Tokenizer.Tokenize(input);
}