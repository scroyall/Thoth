namespace Thoth.Tokenization.Tests;

using Tokens;

[Parallelizable]
public class LiteralTests
{
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(255)]
    [TestCase(32767)]
    [TestCase(int.MaxValue)]
    public void ParsesInteger(int value)
    {
        var tokenized = new Tokenizer(value.ToString()).Tokenize();
        
        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).Matches<IntegerLiteralToken>(t => (t.Value == value)), "Expected token value to match the integer.");
    }
}
