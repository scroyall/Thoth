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

    [Test]
    public void List_IndexesMultidimensionalArrays()
    {
        var result = CompileAndRunSource(@"
            list<int> oned = list<int>[1, 2, 3];
            list<list<int>> twod = list<list<int>>[oned, list<int>[4, 5, 6], oned];

            for (x in 0..2)
            {
                for (y in 0..2)
                {
                    print(twod[x][y]);
                }
            }
        ");

        Assert.That(result.ExitCode, Is.EqualTo(0), "Expected exit with success.");
        Assert.That(result.Output, Is.EqualTo("123456123"));
    }
}
