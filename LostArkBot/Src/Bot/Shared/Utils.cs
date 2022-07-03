using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
