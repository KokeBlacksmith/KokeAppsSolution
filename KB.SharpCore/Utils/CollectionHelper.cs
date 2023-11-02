using System.Collections;
using System.Linq;

namespace KB.SharpCore.Utils;

public static class CollectionHelper
{
    public static bool IsEmpty(ICollection? enumerable)
    {
        return enumerable == null || enumerable.Count == 0;
    }
    
    public static bool HasAny(ICollection? enumerable)
    {
        return !CollectionHelper.IsEmpty(enumerable);
    }

    public static string? StringArrayToNewLinesString(string[]? array)
    {
        if(CollectionHelper.HasAny(array))
        {
            return String.Join(Environment.NewLine, array!);
        }
        else
        {
            return null;
        }
    }
}