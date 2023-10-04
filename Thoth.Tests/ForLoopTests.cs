namespace Thoth.Tests;

public class ForLoopTests
    : CompilerTests
{
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
}
