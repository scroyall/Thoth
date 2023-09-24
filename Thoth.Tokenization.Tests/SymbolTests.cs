namespace Thoth.Tokenization.Tests;

using Tokens;

[Parallelizable]
public class SymbolTests
{
    [Test]
    public void ParsesSymbol([Values] SymbolType symbol)
    {
        if (symbol == SymbolType.Hash)
        {
            Assert.Ignore("Hash symbols won't parse to a token because comments are completely ignored.");
        }

        if (symbol == SymbolType.DoubleQuote)
        {
            Assert.Ignore("Double quotes won't parse to a token because they form string literals.");
        }

        var tokenized = new Tokenizer($"{symbol.ToCharacter()}").Tokenize();

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).Matches<SymbolToken>(t => (t.Type == symbol)), $"Expected symbol token of type {symbol}.");
    }
}
