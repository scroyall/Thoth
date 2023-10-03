using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

[Parallelizable]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class ParserTests
{
    protected Parser Parser { get; } = new();

    protected TokenizedProgramFaker Program { get; } = new();

    protected ParsedProgram Parse()
        => Parser.Parse(Program.ToTokenizedProgram());
}
