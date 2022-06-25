using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class WhenModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("when", "Shows the time of the lfg if set")]
        public async Task When()
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

            Embed embed = message.Embeds.First() as Embed;
            EmbedField timeField = new();

            foreach (EmbedField field in embed.Fields)
            {
                if (field.Name == "Time")
                {
                    timeField = field;
                }
            }

            if (string.IsNullOrEmpty(timeField.Name))
            {
                await RespondAsync(text: "This event doesn't have a time set", ephemeral: true);

                return;
            }

            long unixSeconds = long.Parse(timeField.Value.Replace("<t:", "").Replace(":F>", ""));

            await RespondAsync($"The event starts at <t:{unixSeconds}:F>\n\nThat's in <t:{unixSeconds}:R>");
        }
    }
}
