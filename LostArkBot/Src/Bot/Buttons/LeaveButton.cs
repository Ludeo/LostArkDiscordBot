using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    internal class LeaveButton
    {
        public static async Task Leave(SocketMessageComponent component)
        {
            Embed originalEmbed = component.Message.Embeds.First();
            string userMention = component.User.Mention;

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

            bool userLeft = false;

            foreach (EmbedField embedField in originalEmbed.Fields)
            {
                string content = embedField.Value;

                if (content.Contains(userMention))
                {
                    userLeft = true;
                    continue;
                }
                if (embedField.Name.Contains("Custom Message") || embedField.Name.Contains("Time"))
                {
                    newEmbed.AddField(embedField.Name, embedField.Value, false);
                }
                else
                {
                    newEmbed.AddField(embedField.Name, embedField.Value, true);
                }
            }

            if (userLeft)
            {

                string title = originalEmbed.Title;
                string title1 = title.Split("(")[1];
                string title2 = title1.Split(")")[0];
                string playerNumberJoined = title2.Split("/")[0];
                string playerNumberMax = title2.Split("/")[1];
                newEmbed.Title = $"{title.Split("(")[0]}({int.Parse(playerNumberJoined) - 1}/{playerNumberMax})";

                await component.UpdateAsync(x => x.Embed = newEmbed.Build());

                List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
                ThreadLinkedMessage linkedMessage = threadLinkedMessageList.First(x => x.MessageId == component.Message.Id);

                IThreadChannel threadChannel = Program.Client.GetChannel(linkedMessage.ThreadId) as IThreadChannel;
                await threadChannel.RemoveUserAsync(component.User as IGuildUser);
            } else
            {
                await component.RespondAsync(text: "You are not part of this event", ephemeral: true);
            }
        }
    }
}