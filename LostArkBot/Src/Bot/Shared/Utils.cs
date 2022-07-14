using Newtonsoft.Json;
using System;
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

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    
        public static DateTime? TryParseDateString(string dateString)
        {
            bool res;
            DateTime dtParsed = new();

            res = DateTime.TryParseExact(dateString, "dd/MM HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            if (!res)
            {
                res = DateTime.TryParseExact(dateString, "d/M H:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            }
            if (!res)
            {
                res = DateTime.TryParseExact(dateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            }
            if (!res)
            {
                res = DateTime.TryParseExact(dateString, "d/M/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            }
            if (!res)
            {
                res = DateTime.TryParseExact(dateString, "dd.MM HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            }
            if (!res)
            {
                res = DateTime.TryParseExact(dateString, "d.M H:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            }
            if (!res)
            {
                res = DateTime.TryParseExact(dateString, "dd.MM. HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            }
            if (!res)
            {
                res = DateTime.TryParseExact(dateString, "d.M. H:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            }
            if (!res)
            {
                res = DateTime.TryParseExact(dateString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            }
            if (!res)
            {
                res = DateTime.TryParseExact(dateString, "d.M.yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dtParsed);
            }

            if (!res)
            {
                return null;
            }

            return dtParsed;
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
