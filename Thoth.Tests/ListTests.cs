namespace Thoth.Tests;

public class ListTests
    : CompilerTests
{
    [Test]
    public void List_IndexesWithIntegerLiteral()
    {
        var result = CompileAndRunSource(@"
            list<int> numbers = list<int>[1, 2, 3];

            assert(numbers[0] == 1);
            assert(numbers[1] == 2);
            assert(numbers[2] == 3);
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with success.");
    }

    [Test]
    public void List_IndexesWithVariable()
    {
        var result = CompileAndRunSource(@"
            list<int> numbers = list<int>[1, 2, 3];

            for (i in 0..2)
            {
                assert(numbers[i] == i + 1);
            }
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with success.");
    }
}
