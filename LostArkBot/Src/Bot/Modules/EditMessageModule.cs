using Discord;
using Discord.Net;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class EditMessageModule
    {
        public static async Task EditMessageAsync(SocketSlashCommand command)
        {
            string messageIdRaw = command.Data.Options.First(x => x.Name == "message-id").Value.ToString();
            ulong messageId = ulong.Parse(messageIdRaw);
            string customMessage = command.Data.Options.First(x => x.Name == "custom-message").Value.ToString();
            ulong userId = command.User.Id;
            ITextChannel channel = Program.Client.GetChannel(command.Channel.Id) as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(messageId);
            IUserMessage message = messageRaw as IUserMessage;
            ulong authorId = message.Interaction.User.Id;

            if (userId != authorId)
            {
                await command.RespondAsync(text: "Only the Author of the Event can change the custom message", ephemeral: true);

                return;
            }

            Embed originalEmbed = message.Embeds.First() as Embed;

            EmbedBuilder newEmbed = new EmbedBuilder()
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

            try
            {
                await command.RespondAsync(text: "Custom Message updated", ephemeral: true);
            }
            catch (HttpException exception)
            {
            }
        }
    }
}