using Thoth.Parsing;
using Thoth.Parsing.Statements;

namespace Thoth.Transpilation.Tests;

[Parallelizable]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class TranspilerTests
{
    protected Transpiler Transpiler { get; } = new TestTranspiler();

    // TODO Remove and use [Values] instead.
    protected static IEnumerable<BasicType> Types => Enum.GetValues<BasicType>();

    /// <summary>
    /// Transpile a parsed program, discarding the output.
    /// </summary>
    protected void Transpile(ParsedProgram program)
        => Transpiler.Transpile(program, Stream.Null);

    protected void Transpile(params Statement[] statements)
        => Transpile(Fakes.Program(statements));
}