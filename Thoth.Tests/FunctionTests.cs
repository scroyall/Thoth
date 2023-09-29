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

    [Test]
    public void Function_WithReturnType_ReturnsValue()
    {
        var result = CompileAndRunSource(@"
            function getNumber() -> int
            {{
                return 1234567890;
            }}

            var number = getNumber();

            assert(number == 1234567890);
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with code zero.");
    }

    [Test]
    public void FunctionCall_WithinFunctionCall_ReturnsCorrectValue()
    {
        var result = CompileAndRunSource(@"
            function outer() -> int
            {{
                return inner() + 1;
            }}

            function inner() -> int
            {{
                return 123;
            }}

            assert(outer() == inner() + 1);
            assert(outer() == 124);
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with code zero.");
    }
}