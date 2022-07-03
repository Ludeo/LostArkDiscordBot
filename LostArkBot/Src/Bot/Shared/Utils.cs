using Newtonsoft.Json;
using System.Globalization;

namespace LostArkBot.Src.Bot.Shared
{
    public static class Utils
    {
        public static T DeepCopy<T>(T objectToCopy)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(objectToCopy));
        }
    }

    public static class StringExtension
    {
        public static string ToTitleCase(this string str)
        {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(str);
        }
    }
}