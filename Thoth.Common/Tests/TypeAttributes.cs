using NUnit.Framework.Interfaces;
using System.Collections;

namespace Thoth.Tests;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple=true, Inherited=false)]
public class TypesAttribute(string LowerBound = "", string UpperBound = "")
    : Attribute, IParameterDataSource
{
    public IEnumerable GetData(IParameterInfo parameter)
    {
        return TypeValueSources.Any(
            lowerBound: BasicType.TryParse(LowerBound),
            upperBound: BasicType.TryParse(UpperBound)
        );
    }
}

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple=true, Inherited=false)]
public class ResolvedTypesAttribute(string LowerBound = "", string UpperBound = "")
    : Attribute, IParameterDataSource
{
    public IEnumerable GetData(IParameterInfo parameter)
    {
        return TypeValueSources.Resolved(
            lowerBound: BasicType.TryParse(LowerBound),
            upperBound: BasicType.TryParse(UpperBound)
        );
    }
}
