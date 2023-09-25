using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

[Parallelizable]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class ParserTests
{
    protected Parser Parser { get; } = new();

    protected ParsedProgram Parse(TokenizedProgram program)
    {
        return Parser.Parse(program);
    }

    protected ParsedProgram Parse(string input)
    {
        return Parse(Fakes.Program(input));
    }
}