namespace Thoth.Tokenizer.Tests;

[Parallelizable]
public class InputTests
{
    [Test]
    public void ParsesEmptyInput()
    {
        var tokenized = new Tokenizer(string.Empty).Tokenize();

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(0), "Expected tokenizer to produce no tokens.");
    }
}
