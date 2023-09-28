namespace Thoth.Transpilation.Tests;

[Parallelizable]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class TranspilerTests
{
    protected Transpiler Transpiler { get; } = new TestTranspiler();

    protected FakeParsedProgram Program { get; } = new();

    /// <summary>
    /// Transpile a parsed program, discarding the output.
    /// </summary>
    protected void Transpile()
        => Transpiler.Transpile(Program.ToParsedProgram(), Stream.Null);
}