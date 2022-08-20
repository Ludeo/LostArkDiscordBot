using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class LocalTimeModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("localtime", "Shows the given server time in your local time")]
        public async Task LocalTime([Summary("server-time", "Server time, Format: HH:mm (24h)")] string serverTime)
        {
            await DeferAsync();

            int hour = int.Parse(serverTime[..2]);
            int minute = int.Parse(serverTime.Substring(3, 2));

            DateTimeOffset now = DateTimeOffset.Now;
            DateTimeOffset dateTimeOffset = new(now.Year, now.Month, now.Day, hour, minute, now.Second, new TimeSpan(1, 0, 0));

            if(dateTimeOffset.ToUnixTimeMilliseconds() < now.ToUnixTimeMilliseconds())
            {
                dateTimeOffset = dateTimeOffset.AddDays(1);
            }

            string formatedServerTime = dateTimeOffset.ToString("HH:mm");

            await FollowupAsync(text: $"Server time: {formatedServerTime}\nLocal time: <t:{dateTimeOffset.ToUnixTimeSeconds()}:t>\n<t:{dateTimeOffset.ToUnixTimeSeconds()}:R>");
        }
    }
}