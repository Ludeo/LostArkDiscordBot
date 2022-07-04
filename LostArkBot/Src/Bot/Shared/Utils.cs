using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace LostArkBot.Src.Bot.Shared
{
    public static class Utils
    {
        public static T DeepCopy<T>(T objectToCopy)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(objectToCopy));
        }

        public static string ParseEngravings(string engravings)
        {
            List<string> splitEngravings = new();
            if (engravings.Contains(","))
            {
                splitEngravings = engravings.Split(",").ToList();
            }
            else if (engravings.Contains("\\"))
            {
                splitEngravings = engravings.Split("\\").ToList();
            }
            else if (engravings.Contains("/"))
            {
                splitEngravings = engravings.Split("/").ToList();
            }
            else
            {
                MatchCollection matches;
                if (char.IsDigit(engravings.TrimStart()[0]))
                {
                    matches = Regex.Matches(engravings, "(\\d)");
                }
                else
                {
                    matches = Regex.Matches(engravings, "([[a-zA-Z\\s]+\\d)");
                }

                foreach (Match match in matches)
                {
                    splitEngravings.Add(match.ToString());
                }
            }

            List<string> parsedEngravings = new();
            foreach (string engraving in splitEngravings)
            {
                parsedEngravings.Add(engraving.Trim().ToTitleCase());
            }

            return string.Join(",", parsedEngravings);
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