using Thoth.Tokenization;

namespace Thoth.Parsing.Tests;

public static class SymbolValueSources
{
    public static IEnumerable<SymbolType> Symbols
        => Enum.GetValues<SymbolType>();
}
