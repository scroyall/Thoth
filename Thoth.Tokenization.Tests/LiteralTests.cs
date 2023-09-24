using Thoth.Tokenization.Tokens;

namespace Thoth.Tokenization.Tests;

public class LiteralTests
    : TokenizerTests
{
    [TestCase(0)]
    [TestCase(+1)]
    [TestCase(-1)]
    [TestCase(+255)]
    [TestCase(-255)]
    [TestCase(+32767)]
    [TestCase(-32767)]
    [TestCase(int.MaxValue)]
    [TestCase(int.MinValue)]
    public void ParsesIntegerLiteralToken_FromIntegerLiteral(int value)
    {
        var tokenized = Tokenize(value.ToString());
        
        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).TypeOf<IntegerLiteralToken>()
                                         .With.Property("Value").EqualTo(value),
                                         "Expected token value to match the integer.");
    }

    public static IEnumerable<TestCaseData> Strings()
    {
        yield return new TestCaseData(string.Empty).SetName("{m}(Empty)");

        // Symbols
        foreach (var symbol in Enum.GetValues<SymbolType>())
        {
            // Double quote would end the string literal.
            if (symbol == SymbolType.DoubleQuote) continue;

            yield return new TestCaseData($"{symbol.ToCharacter()}").SetName($"{{m}}({symbol})");
        }
        
        // Whitespace
        foreach (var whitespace in Enum.GetValues<WhiteSpaceType>())
        {
            yield return new TestCaseData($"{whitespace.ToCharacter()}").SetName($"{{m}}({whitespace})");
        }

        yield return new TestCaseData("Hello, world!");
    }

    [TestCaseSource(nameof(Strings))]
    public void ParsesStringLiteralToken_FromStringLiteral(string value)
    {
        var tokenized = Tokenize($"\"{value}\"");

        Assert.That(tokenized.Tokens, Has.Count.EqualTo(1), "Expected exactly one token.");
        Assert.That(tokenized.Tokens, Has.Exactly(1).TypeOf<StringLiteralToken>()
                                         .With.Property("Index").GreaterThanOrEqualTo(0),
                                         "Expected string literal index to be greater than or equal to zero!");
 
    }

    public void AddsExactlyOneEntryToStringTable_ForStringLiteral(string value)
    {
        var tokenized = Tokenize($"\"{value}\"");

        Assert.That(tokenized.Strings, Has.Exactly(1).EqualTo(value));
    }

    public void AddsValueToStringTable_FromStringLiteral(string value)
    {
        var tokenized = Tokenize($"\"{value}\"");

        Assert.That(tokenized.Strings, Has.Exactly(1).EqualTo(value));

        var expectedIndex = tokenized.Strings.ToList().IndexOf(value);
        Assert.That(tokenized.Tokens, Has.Exactly(1).With.Property("Index").EqualTo(expectedIndex));
    }
}