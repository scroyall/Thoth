using Thoth.Parsing.Expressions;
using Thoth.Parsing.Statements;

namespace Thoth.Parsing.Tests;

[Parallelizable]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class ParserTests
{
    protected Parser Parser { get; } = new();

    protected TokenizedProgramFaker Program { get; } = new();

    protected ParsedProgram Parse()
        => Parser.Parse(Program.ToTokenizedProgram());

    protected Expression ParseAssignmentValue()
    {
        var parsed = Parse();

        Assert.That(parsed.Statements, Has.Count.EqualTo(1).And.One.TypeOf<AssignmentStatement>());

        var assignment = parsed.Statements[0] as AssignmentStatement ?? throw new NullReferenceException();

        return assignment.Value;
    }
}
