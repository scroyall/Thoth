namespace Thoth.Transpilation;

public enum VariableScope
{
    Local,
    Parameter
}

public record DefinedVariable(VariableScope Scope, BasicType Type, int Offset);
