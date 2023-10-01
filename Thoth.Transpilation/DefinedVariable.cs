namespace Thoth.Transpilation;

public enum VariableScope
{
    Local,
    Parameter
}

public record DefinedVariable(VariableScope Scope, IResolvedType Type, int Offset);
