using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;

namespace LostArkBot.Bot.SlashCommands;

public class LocalTimeModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    [SlashCommand("localtime", "Shows the given server time in your local time")]
    public async Task LocalTime([Summary("server-time", "Server time, Format: HH:mm (24h)")] string serverTime)
    {
        await this.DeferAsync();

        int hour = int.Parse(serverTime[..2]);
        int minute = int.Parse(serverTime.Substring(3, 2));

        DateTimeOffset now = DateTimeOffset.Now;
        DateTimeOffset dateTimeOffset = new(now.Year, now.Month, now.Day, hour, minute, now.Second, StaticObjects.TimeOffset);

        if (dateTimeOffset.ToUnixTimeMilliseconds() < now.ToUnixTimeMilliseconds())
        {
            dateTimeOffset = dateTimeOffset.AddDays(1);
        }

        string formatedServerTime = dateTimeOffset.ToString("HH:mm");

        await this.FollowupAsync(
                                 $"Server time: {formatedServerTime}\nLocal time: <t:{dateTimeOffset.ToUnixTimeSeconds()}:t>\n<t:{dateTimeOffset.ToUnixTimeSeconds()}:R>");
    }
}