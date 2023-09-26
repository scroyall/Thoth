using Thoth.Tokenization.Tokens;

namespace Thoth.Tokenization.Tests;

public class BooleanLiteralTests
    : TokenizerTests
{
    [Test]
    public void Parses_TrueBooleanLiteral()
    {
        var tokenized = Tokenize("true");

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).TypeOf<BooleanLiteralToken>()
                                                    .With.Property("Value").EqualTo(true),
                                                    "Expected boolean literal token with value of true.");
    }

    [Test]
    public void Parses_FalseBooleanLiteral()
    {
        var tokenized = Tokenize("false");

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).TypeOf<BooleanLiteralToken>()
                                                    .With.Property("Value").EqualTo(false),
                                                    "Expected boolean literal token with value of false.");
    }
}