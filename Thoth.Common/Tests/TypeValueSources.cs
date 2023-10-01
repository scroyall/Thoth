namespace Thoth.Tests;

public class TypeValueSources
{
    private static IEnumerable<IType> All
        => BasicType.Values;
    
    public static IEnumerable<IType> Any(IType? lowerBound = null, IType? upperBound = null)
    {
        var result = All;
        
        if (lowerBound is not null)
        {
            result = result.Where(t => t.Matches(lowerBound));
        }

        if (upperBound is not null)
        {
            result = result.Where(t => !t.Matches(upperBound));
        }

        return result;
    }

    public static IEnumerable<IResolvedType> Resolved(IType? lowerBound = null, IType? upperBound = null)
    {
        var result = All.Where(t => t.IsResolved()).Select(t => t.Resolve());

        if (lowerBound is not null)
        {
            result = result.Where(t => t.Matches(lowerBound));
        }

        if (upperBound is not null)
        {
            result = result.Where(t => !t.Matches(upperBound));
        }

        return result;
    }
}
