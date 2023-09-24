namespace Thoth.Parsing.Tests;

public abstract class ParserTests
{
    private Parser? _parser;

    protected Parser Parser => _parser ?? throw new NullReferenceException();

    [SetUp]
    public void SetUp()
    {
        _parser = new Parser();
    }
}