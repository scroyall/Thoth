namespace Thoth.Tests;

public class FileTests
    : CompilerTests
{
    private static IEnumerable<string> Files(string directory)
    {
        return Directory.EnumerateFiles(directory);
    }

    private static IEnumerable<TestCaseData> CompilationTestCases()
    {
        var directory = Path.Join(TestContext.CurrentContext.TestDirectory, "TestCases", "Compilation");

        foreach (var path in Directory.EnumerateFiles(directory))
        {
            var name = Path.GetFileNameWithoutExtension(path);

            yield return new TestCaseData(path) 
            {
                TestName = $"{{m}}({name})"
            };
        }
    }

    private static IEnumerable<TestCaseData> ExitCodeTestCases()
    {
        var directory = Path.Join(TestContext.CurrentContext.TestDirectory, "TestCases", "ExitCode");
        foreach (var filePath in Directory.EnumerateFiles(directory))
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var exitCode = int.Parse(fileName.Split('_').Last());

            yield return new TestCaseData(filePath).SetName($"{{m}}({fileName})").Returns(exitCode);
        }
    }

    [Test]
    [TestCaseSource(nameof(CompilationTestCases))]
    public void CompilesAndRuns(string path)
    {
        Assert.That(CompileAndRunFile(path).ExitCode, Is.EqualTo(0), "Expected exit with code zero.");
    }

    [Test]
    [TestCaseSource(nameof(ExitCodeTestCases))]
    public int CompilesAndExitsWithCode(string path)
    {
        return CompileAndRunFile(path).ExitCode;
    }
}
