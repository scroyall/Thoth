using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

[Parallelizable]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class ParserTests
{
    protected Parser Parser { get; } = new();

    protected ParsedProgram Parse(TokenizedProgram program)
        => Parser.Parse(program);

    protected ParsedProgram Parse(string input)
        => Parse(Fakes.Program(input));

    protected ParsedProgram Parse(params Token[] tokens)
        => Parse(Fakes.Program(tokens));

    protected ParsedProgram Parse(IEnumerable<Token> tokens)
        => Parse(Fakes.Program(tokens));
}