using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class WhenModule
    {
        public static async Task WhenAsync(SocketSlashCommand command)
        {
            if (command.Channel.GetChannelType() != ChannelType.PublicThread)
            {
                await command.RespondAsync(text: "This command is only available in the thread of the lfg event", ephemeral: true);

                return;
            }

            List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
            ThreadLinkedMessage linkedMessage = threadLinkedMessageList.First(x => x.ThreadId == command.Channel.Id);
            ulong messageId = linkedMessage.MessageId;

            ITextChannel channel = Program.Client.GetChannel(Config.Default.LfgChannel) as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(messageId);
            IUserMessage message = messageRaw as IUserMessage;

            Embed embed = message.Embeds.First() as Embed;

            if(embed.Timestamp == null)
            {
                await command.RespondAsync(text: "This event doesn't have a time set", ephemeral: true);

                return;
            }

            DateTimeOffset time = embed.Timestamp.Value;
            DateTimeOffset now = DateTimeOffset.Now;
            TimeSpan difference = time - now;

            await command.RespondAsync("The event starts at " + time.ToString("MM/dd hh:mm tt") + "\n\nThat's in " + difference.Hours + " hours and " + difference.Minutes + " minutes",
                ephemeral: true);
        }
    }
}
