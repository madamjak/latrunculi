using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Latrunculi.Extensions
{
    public static class EnumerableExtensions
    {
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = new Random().Next(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
}
