using System.Collections;
using System.Linq;

namespace KB.SharpCore.Utils;

public static class CollectionHelper
{
    public static bool IsEmpty(ICollection? enumerable)
    {
        return enumerable is { Count: 0 };
    }
    
    public static bool HasAny(ICollection? enumerable)
    {
        return !CollectionHelper.IsEmpty(enumerable);
    }
}