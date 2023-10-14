using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public class ClassDefinitionTests
    : ParserTests
{
    [Test]
    public void ClassDefinition_WithoutMembers_Parses()
    {
        Program.KeywordToken(KeywordType.Class);
        var className = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        var parsed = Parse();

        Assert.That(parsed.Classes, Contains.Key(className).And.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var klass = parsed.Classes[className] ?? throw new NullReferenceException();
            Assert.That(klass.Name, Is.EqualTo(className));
        });
    }

    [Test]
    public void ClassDefinition_WhenAlreadyDefined_ThrowsException()
    {
        Program.KeywordToken(KeywordType.Class);
        var className = Program.IdentifierToken().Name;
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        Program.KeywordToken(KeywordType.Class);
        Program.IdentifierToken(className);
        Program.SymbolToken(SymbolType.LeftBrace);
        Program.SymbolToken(SymbolType.RightBrace);

        Assert.Throws<MultiplyDefinedClassException>(() => Parse());
    }
}
