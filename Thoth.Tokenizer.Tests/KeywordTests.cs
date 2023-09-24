namespace Thoth.Tokenizer.Tests;

using Tokens;

[Parallelizable]
public class KeywordTests
{
    [Test]
    public void ParsesKeyword([Values] KeywordType keyword)
    {
        var tokenized = new Tokenizer(keyword.ToIdentifier()).Tokenize();

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).Matches<KeywordToken>(t => (t.Type == keyword)), $"Expected keyword token of type {keyword}.");
    }
}
