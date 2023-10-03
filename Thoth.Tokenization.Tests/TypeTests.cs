using Thoth.Tokenization.Tokens;

namespace Thoth.Tokenization.Tests;

public class TypeTests
    : TokenizerTests
{
    [Test]
    public void ParsesBuiltInTypes([Values] BuiltinType type)
    {
        var tokenized = Tokenize($"{type.ToString().ToLower()}");

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).TypeOf<BuiltinTypeToken>()
                                         .With.Property("Type").EqualTo(type));
    }
}