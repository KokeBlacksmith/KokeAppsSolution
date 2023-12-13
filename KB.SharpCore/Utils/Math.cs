using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.SharpCore.Utils;

public static class Math
{
    public static bool IsBetween<T>(this T item, T start, T end)
        where T : IComparable, IComparable<T>
    {
        return Comparer<T>.Default.Compare(item, start) >= 0 && Comparer<T>.Default.Compare(item, end) <= 0;
    }
}
