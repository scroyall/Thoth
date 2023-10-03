using NUnit.Framework.Interfaces;
using System.Collections;

namespace Thoth.Tests;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple=true, Inherited=false)]
public class TypesAttribute(string? Except = null)
    : Attribute, IParameterDataSource
{
    public IEnumerable GetData(IParameterInfo parameter)
    {
        var result = TypeValueSources.All;

        if (Except is not null)
        {
            var excluded = Enum.Parse<BuiltinType>(Except);
            result = result.Where(t => t.Root != excluded);
        }

        return result;
    }
}
