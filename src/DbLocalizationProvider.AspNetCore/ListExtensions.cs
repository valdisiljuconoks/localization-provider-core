using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.AspNetCore {
    public static class ListExtensions
    {
        public static void Deconstruct<T>(this List<T> list, out T head, out List<T> tail)
        {
            head = list.FirstOrDefault();
            tail = new List<T>(list.Skip(1));
        }
    }
}