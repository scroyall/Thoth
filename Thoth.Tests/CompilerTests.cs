using System.Diagnostics;
using System.Text;
using Thoth.Parsing;
using Thoth.Tokenization;
using Thoth.Transpilation;
using Thoth.Utils;

namespace Thoth.Tests;

public abstract class CompilerTests
{
    public record RunResult(int ExitCode, string Output);

    private static string OutputDirectory => Path.Join(TestContext.CurrentContext.TestDirectory, "TestOutput");

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        if (Directory.Exists(OutputDirectory))
        {
            Directory.Delete(OutputDirectory, true);            
        }

        Directory.CreateDirectory(OutputDirectory);
    }

    protected RunResult CompileAndRunFile(string path)
        => CompileAndRunSource(File.ReadAllText(path));

    protected RunResult CompileAndRunSource(string source)
    {
        var tokenized = new Tokenizer().Tokenize(source);
        var parsed = new Parser().Parse(tokenized);
        var transpiler = new Transpiler();


        var fileName = DateTime.Now.Ticks.ToString();
        var asmFilePath = Path.Join(OutputDirectory, $"{fileName}.asm");
        using (var stream = new FileStream(asmFilePath, FileMode.CreateNew))
        {
            transpiler.Transpile(parsed, stream);
        }

        var executableFilePath = Compilation.CompileAndLink(asmFilePath);
        var executableDirectoryPath = Path.GetDirectoryName(executableFilePath);

        var process = new Process();
        process.StartInfo.FileName = executableFilePath;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.WorkingDirectory = executableDirectoryPath;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit(TimeSpan.FromSeconds(1));

        return new(process.ExitCode, output);
    }
}