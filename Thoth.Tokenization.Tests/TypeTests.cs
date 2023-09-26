using Thoth.Tokenization.Tokens;

namespace Thoth.Tokenization.Tests;

public class TypeTests
    : TokenizerTests
{
    [Test]
    public void ParsesType([Values] BasicType type)
    {
        var tokenized = Tokenize($"{type.ToSourceString()}");

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).TypeOf<TypeToken>()
                                         .With.Property("Type").EqualTo(type));
    }
}