using Discord;
using Discord.Net;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Menus
{
    internal class KickCharacterMenu
    {
        public static async Task KickCharacter(SocketMessageComponent component)
        {
            string messageIdRaw = component.Data.Values.First();
            string messageIdString = messageIdRaw.Split(",")[0];
            ulong messageId = ulong.Parse(messageIdString);
            ITextChannel channel = Program.Client.GetChannel(component.Message.Channel.Id) as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(messageId);
            IUserMessage message = messageRaw as IUserMessage;
            Embed originalEmbed = messageRaw.Embeds.First() as Embed;
            string characterName = messageIdRaw.Split(",")[1];

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

            foreach (EmbedField field in originalEmbed.Fields)
            {
                if (field.Value.Split("\n")[1][5..] == characterName)
                {
                    string title = originalEmbed.Title;
                    string title1 = title.Split("(")[1];
                    string title2 = title1.Split(")")[0];
                    string playerNumberJoined = title2.Split("/")[0];
                    string playerNumberMax = title2.Split("/")[1];
                    newEmbed.Title = $"{title.Split("(")[0]}({int.Parse(playerNumberJoined) - 1}/{playerNumberMax})";

                    continue;
                }

                newEmbed.AddField(new EmbedFieldBuilder().WithName(field.Name).WithValue(field.Value).WithIsInline(field.Inline));
            }

            await message.ModifyAsync(x =>
            {
                x.Embed = newEmbed.Build();
            });

            try
            {
                await component.RespondAsync();
            }
            catch (HttpException exception)
            {
                await Program.Log(new LogMessage(LogSeverity.Error, "KickCharacterMenu.cs", exception.Message));
            }
        }
    }
}