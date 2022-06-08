using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class EditMessageModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("editmessage", "Edits the message of the LFG")]
        public async Task EditMessage([Summary("custom-message", "New custom message for the event")] string customMessage)
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
            ulong authorId = message.Interaction.User.Id;

            if (Context.User.Id != authorId && !Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                await RespondAsync(text: "Only the Author of the Event can change the custom message", ephemeral: true);

                return;
            }

            Embed originalEmbed = message.Embeds.First() as Embed;

            EmbedBuilder newEmbed = new()
            {
                Title = originalEmbed.Title,
                Description = originalEmbed.Description,
                Author = new EmbedAuthorBuilder
                {
                    Name = originalEmbed.Author!.Value.Name,
                    IconUrl = originalEmbed.Author!.Value.IconUrl,
                },
                ThumbnailUrl = originalEmbed.Thumbnail.Value.Url,
                ImageUrl = originalEmbed.Image.Value.Url,
                Color = originalEmbed.Color.Value,
            };

            if (originalEmbed.Timestamp != null)
            {
                newEmbed.Timestamp = originalEmbed.Timestamp.Value;
            }

            newEmbed.AddField("Custom Message", customMessage, false);

            foreach (EmbedField field in originalEmbed.Fields)
            {
                if (field.Name == "Custom Message")
                {
                    continue;
                }

                newEmbed.AddField(field.Name, field.Value, field.Inline);
            }

            await message.ModifyAsync(x => x.Embed = newEmbed.Build());
            await RespondAsync(text: "Custom Message updated", ephemeral: true);
        }
    }
}