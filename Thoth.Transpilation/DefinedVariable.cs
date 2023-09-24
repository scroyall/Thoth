using Thoth.Parsing;

namespace Thoth.Transpilation;

public readonly struct DefinedVariable(BasicType type, int index)
{
    public readonly BasicType Type = type;
    public readonly int Index = index;
}