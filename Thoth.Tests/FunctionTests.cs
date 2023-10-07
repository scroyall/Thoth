namespace Thoth.Tests;

public class FunctionTests
    : CompilerTests
{
    [Test]
    public void Function_WithoutParametersOrReturnType_ExecutesStatements()
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

            assert(getNumber() == 1234567890);
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

    [Test]
    public void FunctionCall_PassesSingleParameter()
    {
        var result = CompileAndRunSource(@"
            function assertpositive(int value)
            {{
                assert(value > 0);
            }}

            # Integer Literal
            assertpositive(1);

            # Integer Expression
            assertpositive(-1 + 2);

            # Integer Variable
            int positive = 1;
            assertpositive(positive);

            function asserttrue(bool value)
            {{
                assert(value);
            }}

            # Boolean Literal
            asserttrue(true);

            # Boolean Expression
            asserttrue(!false);

            # Boolean Variable
            bool truth = true;
            asserttrue(truth);
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with code zero.");
    }

    [Test]
    public void FunctionCall_PassesMultipleParameters()
    {
        var result = CompileAndRunSource(@"
            function aplusbequalsc(int a, int b, int c)
            {{
                assert(a + b == c);
            }}

            # Integer Literals
            aplusbequalsc(1, 2, 3);

            # Integer Expressions
            aplusbequalsc(1 + 2, 3 + 4, 5 + 6 - 1);

            # Integer Variables
            int a = 1;
            int b = 2;
            int c = 3;
            aplusbequalsc(a, b, c);

            function alltrue(bool a, bool b, bool c)
            {{
                assert(a && b && c);
            }}

            # Boolean Literals
            alltrue(true, true, true);

            # Boolean Expressions
            alltrue(!!true, !false, false == false);

            # Boolean Variables
            bool truth = true;
            bool fact = true;
            bool absolute = true;
            alltrue(truth, fact, absolute);
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with code zero.");
    }

    [Test]
    public void FunctionCall_Recurses()
    {
        var result = CompileAndRunSource(@"
            function factorial(int value) -> int
            {{
                assert(value > 0);
                if (value == 1) return 1;
                return value * factorial(value - 1);
            }}

            assert(factorial(10) == 3628800);
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with code zero.");
    }

    [Test]
    public void Function_AssignsLocalVariables()
    {
        var result = CompileAndRunSource(@"
            function foo()
            {{
                int y = 1;
                int z = 2;
                int w = 3;
                assert(w == 3);
                assert(z == 2);
                assert(y == 1);
            }}

            foo();
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with success.");
    }
}
