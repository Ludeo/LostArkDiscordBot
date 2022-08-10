using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LostArkBot.Src.Bot.Shared
{
    public static class Extensions
    {
        public static string ToTitleCase(this string str)
        {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(str);
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second)
        {

            first = list.Count > 0 ? list[0] : default; // or throw
            second = list.Count > 1 ? list[1] : default; // or throw
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default; // or throw
            second = list.Count > 1 ? list[1] : default; // or throw
            rest = list.Skip(2).ToList();
        }
    }
}
