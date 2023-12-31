namespace Thoth.Tests;

public class ForLoopTests
    : CompilerTests
{
    [Test]
    public void ForLoop_AccessesVariableInContainingScope()
    {
        var result = CompileAndRunSource(@"
            int number = 123456789;

            for (other in 1..10)
            {
                assert(number == 123456789);
            }

            int outside = 0;
            for (other in list<int>[1, 2, 3, 4, 5, 6, 7, 8, 9])
            {
                assert(number == 123456789);

                outside = outside + other;
            }

            assert(outside == 45);
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with success.");
    }

    [Test]
    public void ForLoop_Nested_IteratesCorrectly()
    {
        var result = CompileAndRunSource(@"
            int count = 0;
            int total = 0;

            for (x in 1..3)
            {
                for (y in 1..5)
                {
                    count = count + 1;
                    total = total + x * y;
                }
            }

            assert(count == 15);
            assert(total == 90);
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with success.");
    }

    [Test]
    public void ForLoop_IteratesListLiteral()
    {
        var result = CompileAndRunSource(@"
            for (number in list<int>[8, 6, 7, 5, 3, 0, 9])
            {
                print(number);
            }
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with success.");
        Assert.That(result.Output, Is.EqualTo("8675309"));
    }

    [Test]
    public void ForLoop_IteratesListVariable()
    {
        var result = CompileAndRunSource(@"
            list<int> numbers = list<int>[1, 3, 5, 7, 9];

            for (number in numbers)
            {
                print(number);
            }
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with success.");
        Assert.That(result.Output, Is.EqualTo("13579"));
    }
}
