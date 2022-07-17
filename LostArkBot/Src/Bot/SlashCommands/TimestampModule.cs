using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class TimestampModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("timestamps", "Get copy-able timestamps that display LOCAL time")]
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

            await RespondAsync($"Time only (<t:{utcTime}:t>): ```<t:{utcTime}:t>```" +
                $"Date and time (<t:{utcTime}:F>): ```<t:{utcTime}:F>```" +
                $"Countdown (<t:{utcTime}:R>): ```<t:{utcTime}:R>```",
                ephemeral: true);


        }
    }
}