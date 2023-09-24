namespace Thoth.Tokenization.Tests;

using Tokens;

[Parallelizable]
public class WhitespaceTests
{
    [Test]
    public void IgnoresWhiteSpace([Values] WhiteSpaceType whitespace)
    {
        var tokenized = new Tokenizer($"{whitespace.ToCharacter()}").Tokenize();

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(0), "Expected zero tokens.");
    }

    [Test]
    public void IgnoresWhiteSpaceBeforeKeyword([Values] WhiteSpaceType whitespace, [Values] KeywordType keyword)
    {
        IgnoresWhiteSpaceAroundKeyword("{0}{1}", whitespace, keyword);
    }

    [Test]
    public void IgnoresWhiteSpaceAfterKeyword([Values] WhiteSpaceType whitespace, [Values] KeywordType keyword)
    {
        IgnoresWhiteSpaceAroundKeyword("{1}{0}", whitespace, keyword);
    }

    private static void IgnoresWhiteSpaceAroundKeyword(string format, WhiteSpaceType whitespace, KeywordType keyword)
    {
        var text = string.Format(format, whitespace.ToCharacter(), keyword.ToIdentifier());
        var tokenized = new Tokenizer(text).Tokenize();

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected only one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).Matches<KeywordToken>(t => (t.Type == keyword)), $"Expected keyword token of type {keyword}.");
    }
}
