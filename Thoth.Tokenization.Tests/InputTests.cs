namespace Thoth.Tokenization.Tests;

public class InputTests
    : TokenizerTests
{
    [Test]
    public void ParsesEmptyInput()
    {
        var tokenized = Tokenize(string.Empty);

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(0), "Expected tokenizer to produce no tokens.");
    }
}
