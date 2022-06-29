using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class CalendarModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("calendar", "Exports the date of the event as a ics file so you can import it into your calendar")]
        public async Task EditMessage()
        {
            if (Context.Channel.GetChannelType() != ChannelType.PublicThread)
            {
                await RespondAsync(text: "This command is only available in the thread of the lfg event", ephemeral: true);

                return;
            }

            SocketThreadChannel threadChannel = Context.Channel as SocketThreadChannel;
            ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(threadChannel.Id);
            IUserMessage message = messageRaw as IUserMessage;

            Embed originalEmbed = message.Embeds.First() as Embed;
            string time = string.Empty;

            foreach (EmbedField field in originalEmbed.Fields)
            {
                if(field.Name == "Time")
                {
                    time = field.Value.Split("\n")[0];
                }
            }

            if(string.IsNullOrEmpty(time))
            {
                await RespondAsync(text: "This lfg doesn't have a time set", ephemeral: true);
            }

            long unixSeconds = long.Parse(time.Replace("<t:", "").Replace(":F>", ""));
            DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

            string timeStartFormatted = date.ToString("yyyyMMddTHHmmssZ");
            date = date.AddHours(2);
            string timeEndFormatted = date.ToString("yyyyMMddTHHmmssZ");
            string summary = threadChannel.Name;

            string icsString = "BEGIN:VCALENDAR\nVERSION:2.0\nPRODID:-//Ludeo//Lost Ark Bot//EN\nBEGIN:VEVENT\nDTSTART:" + timeStartFormatted + "\nDTEND:" + timeEndFormatted
                + "\nSUMMARY:" + summary + "\nEND:VEVENT\nEND:VCALENDAR";

            await File.WriteAllTextAsync("DateExport.ics", icsString);
            await RespondWithFileAsync(fileStream: File.OpenRead("DateExport.ics"), fileName: "DateExport.ics", ephemeral: true);
            File.Delete("DateExport.ics");
        }
    }
}