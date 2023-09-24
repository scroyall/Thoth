using Thoth.Tokenization.Tokens;

namespace Thoth.Tokenization.Tests;

public class KeywordTests
    : TokenizerTests
{
    [Test]
    public void ParsesKeyword([Values] KeywordType keyword)
    {
        var tokenized = Tokenize(keyword.ToIdentifier());

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).Matches<KeywordToken>(t => (t.Type == keyword)), $"Expected keyword token of type {keyword}.");
    }
}
