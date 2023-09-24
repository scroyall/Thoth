using System.Diagnostics;

namespace Thoth.Utils;

public static class Compilation
{
    public static string CompileAndLink(string asmPath, int timeoutSeconds = 3)
    {
        return Link(Compile(asmPath, timeoutSeconds), timeoutSeconds);
    }

    private static string Compile(string asmPath, int timeoutSeconds = 3)
    {
        string directory = Path.GetDirectoryName(asmPath) ??
                           throw new Exception($"Failed to get directory name for '{asmPath}'.");

        string name = Path.GetFileName(asmPath) ??
                      throw new Exception($"Failed to get file name without extension for '{asmPath}'.");

        string nameWithoutExtension = Path.GetFileNameWithoutExtension(asmPath) ??
                                      throw new Exception($"Failed to get file name without extension for '{asmPath}'.");

        var info = new ProcessStartInfo()
        {
            WorkingDirectory = directory,
            FileName = "nasm",
            ArgumentList = { "-f elf64", "-g", "-F dwarf", name }
        };

        if (Process.Start(info) is not { } process)
        {
            throw new Exception("Compilation failed; failed to start process.");
        }

        process.WaitForExit(TimeSpan.FromSeconds(timeoutSeconds));

        if (process.ExitCode != 0)
        {
            throw new Exception($"Compilation failed; process exited with code {process.ExitCode}.");
        }

        return Path.Join(directory, nameWithoutExtension + ".o");
    }

    private static string Link(string asmPath, int timeoutSeconds = 3)
    {
        var directory = Path.GetDirectoryName(asmPath) ??
                        throw new Exception($"Failed to get directory name for '{asmPath}'.");

        var name = Path.GetFileName(asmPath) ??
                   throw new Exception($"Failed to get file name without extension for '{asmPath}'.");

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(asmPath) ??
                                   throw new Exception($"Failed to get file name without extension for '{asmPath}'.");

        var info = new ProcessStartInfo()
        {
            WorkingDirectory = directory,
            FileName = "ld",
            Arguments = $"-o {nameWithoutExtension} {name}"
        };

        if (Process.Start(info) is not { } process)
        {
            throw new Exception("Compilation failed; failed to start process.");
        }

        process.WaitForExit(TimeSpan.FromSeconds(timeoutSeconds));

        if (process.ExitCode != 0)
        {
            throw new Exception($"Compilation failed; process exited with code {process.ExitCode}.");
        }

        return Path.Join(directory, nameWithoutExtension);
    }
}
