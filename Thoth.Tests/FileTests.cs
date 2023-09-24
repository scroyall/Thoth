using System.Diagnostics;

using Thoth.Parsing;
using Thoth.Tokenization;
using Thoth.Transpilation;
using Thoth.Utils;

namespace Thoth.Tests;

public class FileTests
{
    private static string OutputDirectory => Path.Join(TestContext.CurrentContext.TestDirectory, "TestOutput");

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

            yield return new TestCaseData(filePath).Returns(exitCode);
        }
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        if (Directory.Exists(OutputDirectory))
        {
            Directory.Delete(OutputDirectory, true);            
        }

        Directory.CreateDirectory(OutputDirectory);
    }

    [Test]
    [TestCaseSource(nameof(CompilationTestCases))]
    public void CompilesAndRuns(string path)
    {
        CompileAndRun(path);
    }

    [Test]
    [TestCaseSource(nameof(ExitCodeTestCases))]
    public int CompilesAndExitsWithCode(string path)
    {
        return CompileAndRun(path);
    }

    private int CompileAndRun(string filePath)
    {
        var inputFileText = File.ReadAllText(filePath);

        var tokenized = new Tokenizer().Tokenize(inputFileText);
        var parsed = new Parser().Parse(tokenized);
        var transpiler = new Transpiler();

        var asmFilePath = Path.Join(OutputDirectory, Path.GetFileNameWithoutExtension(filePath) + ".th");
        using (var file = new FileStream(asmFilePath, FileMode.CreateNew))
        {
            transpiler.Transpile(parsed, file);
        }

        var executableFilePath = Compilation.CompileAndLink(asmFilePath);
        var executableDirectoryPath = Path.GetDirectoryName(executableFilePath);

        var process = Process.Start(new ProcessStartInfo(executableFilePath) { WorkingDirectory = executableDirectoryPath }) ?? throw new Exception("Failed to start process.");
        process.WaitForExit(TimeSpan.FromSeconds(1));
        return process.ExitCode;
    }
}
