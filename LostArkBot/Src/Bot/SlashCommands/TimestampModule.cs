using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class TimestampModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("timestamp", "Get copy-able relative timestamp from server time")]
        public async Task RelativeTimestamp([Summary("server-date", "Server time")] string serverTime)
        {
            DateTime? parsedDate = Utils.TryParseDateString(serverTime);

            if (parsedDate == null)
            {
                await RespondAsync("Incorrect date-time format", ephemeral: true);
                return;
            }

            DateTimeOffset date = (DateTimeOffset)parsedDate;
            date = new(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, new TimeSpan(1, 0, 0));
            long utcTime = date.ToUnixTimeSeconds();

            await RespondAsync($"```<t:{utcTime}:R>```", ephemeral: true);
        }
    }
}