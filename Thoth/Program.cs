using System.Diagnostics;
using Thoth.Parser;
using Thoth.Tokenizer;
using Thoth.Transpiler;
using Thoth.Utils;

if (args.Length != 1)
{
    Console.Error.WriteLine("Expected one argument!");
    return 1;
}

var text = File.ReadAllText(args[0]);
var tokenized = new Tokenizer(text).Tokenize();

Console.WriteLine("Tokens: ");

var line = 0;
foreach (var token in tokenized.Tokens)
{
    if (token.Source.Line > line)
    {
        if (line > 0) Console.WriteLine();
        line = token.Source.Line;

        Console.Write($"\tLine {line}:");
    }
    
    Console.Write($" {token}");
}
Console.WriteLine();

var parsed = new Parser(tokenized).Parse();

Console.WriteLine();
Console.WriteLine("Statements:\n\t" + string.Join("\n\t", parsed.Statements));

var filename = Path.GetFileNameWithoutExtension(args[0]);
var asmPath = Path.Join(Directory.GetCurrentDirectory(), $"{filename}.asm");
using (var stream = File.Create(asmPath))
{
    Console.WriteLine();

    var transpiler = new Transpiler();
    transpiler.Transpile(parsed, stream);
}

Console.WriteLine("Successfully transpiled.");

Compilation.CompileAndLink(asmPath);

return 0;
