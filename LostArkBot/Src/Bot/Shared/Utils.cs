using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LostArkBot.Bot.Shared;

public static class Utils
{
    public static EmbedBuilder CreateProfileEmbed(string characterName, LostArkBotContext dbcontext, Func<Character, SocketGuildUser> getUser)
    {
        characterName = characterName.ToTitleCase();

        Character character = dbcontext
                              .Characters.Where(x => string.Equals(x.CharacterName, characterName, StringComparison.CurrentCultureIgnoreCase))
                              .Include(x => x.User)
                              .FirstOrDefault();

        if (character is null)
        {
            return null;
        }

        EmbedBuilder embedBuilder = new()
        {
            Title = $"Profile of {characterName}",
            ThumbnailUrl = character.ProfilePicture == string.Empty
                ? getUser(character).GetAvatarUrl()
                : character.ProfilePicture,
            Color = new Color(222, 73, 227),
        };

        embedBuilder.AddField("Item Level", character.ItemLevel, true);
        embedBuilder.AddField("Class", character.ClassName, true);

        string[] engravings = character.Engravings.Split(",");
        string engraving = "\u200b";

        foreach (string x in engravings)
        {
            engraving += x + "\n";
        }

        embedBuilder.AddField("Engravings", engraving, true);
        embedBuilder.AddField("Stats", $"Crit: {character.Crit}\nSpec: {character.Spec}\nDom: {character.Dom}", true);
        embedBuilder.AddField("\u200b", $"Swift: {character.Swift}\nEnd: {character.End}\nExp: {character.Exp}", true);

        if (character.CustomProfileMessage != string.Empty)
        {
            embedBuilder.AddField("Custom Message", character.CustomProfileMessage);
        }

        return embedBuilder;
    }

    public static T DeepCopy<T>(T objectToCopy) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(objectToCopy));

    public static string ParseEngravings(string engravings)
    {
        List<string> splitEngravings = new();

        if (engravings.Contains(','))
        {
            splitEngravings = engravings.Split(",").ToList();
        }
        else if (engravings.Contains('\\'))
        {
            splitEngravings = engravings.Split('\\').ToList();
        }
        else if (engravings.Contains('/'))
        {
            splitEngravings = engravings.Split('/').ToList();
        }
        else
        {
            MatchCollection matches = Regex.Matches(engravings, char.IsDigit(engravings.TrimStart()[0]) ? "(\\d)" : "([[a-zA-Z\\s]+\\d)");

            foreach (Match match in matches)
            {
                splitEngravings.Add(match.ToString());
            }
        }

        List<string> parsedEngravings = splitEngravings.Select(engraving => engraving.Trim().ToTitleCase()).ToList();

        return string.Join(",", parsedEngravings);
    }

    public static DateTime? TryParseDateString(string dateString)
    {
        bool res = DateTime.TryParseExact(
                                          dateString,
                                          "dd/MM HH:mm",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.AllowWhiteSpaces,
                                          out DateTime dtParsed);

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

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();

        return dateTime;
    }
}