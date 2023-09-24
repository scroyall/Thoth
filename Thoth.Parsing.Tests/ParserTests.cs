namespace Thoth.Parsing.Tests;

[Parallelizable]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class ParserTests
{
    protected Parser Parser { get; } = new();
}