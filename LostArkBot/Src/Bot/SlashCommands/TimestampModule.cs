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
            DateTime date = (DateTime)parsedDate;
            long utcTime = new DateTimeOffset(date).ToUnixTimeSeconds();

            await RespondAsync($"```<t:{utcTime}:R>```", ephemeral: true);
        }
    }
}
