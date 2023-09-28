namespace Thoth.Tests;

public class FunctionTests
    : CompilerTests
{
    [Test]
    public void SimpleFunction_WhenCalled_ExecutesStatements()
    {
        var result = CompileAndRunSource(@"
            function printNumbers()
            {
                print(0);
                print(1);
            }

            printNumbers();
            printNumbers();
            printNumbers();
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with code zero.");
        Assert.That(result.Output, Is.EqualTo("010101"));
    }
}