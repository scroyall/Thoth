using Thoth.Parsing;
using Thoth.Parsing.Statements;

namespace Thoth.Transpilation.Tests;

[Parallelizable]
public abstract class TranspilerTests
{
    private Transpiler? _transpiler;

    protected Transpiler Transpiler => _transpiler ?? throw new NullReferenceException();

    protected static IEnumerable<BasicType> Types => Enum.GetValues<BasicType>();

    [SetUp]
    public void SetUp()
    {
        _transpiler = new TestTranspiler();
    }

    /// <summary>
    /// Transpile a parsed program, discarding the output.
    /// </summary>
    protected void Transpile(ParsedProgram program)
        => Transpiler.Transpile(program, Stream.Null);

    protected void Transpile(params Statement[] statements)
        => Transpile(Fakes.Program(statements));
}