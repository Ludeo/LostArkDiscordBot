using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Menus
{
    internal class CubeEndMenu
    {
        public static async Task CubeEnd(SocketMessageComponent component, Dictionary<string, string> eventImages)
        {
            string cubeName = component.Data.Values.First();
            string customMessage = component.Message.Embeds.First().Footer == null ? null : component.Message.Embeds.First().Footer.Value.Text;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"[Cube] {cubeName} (1/4)",
                Description = "Waiting for members to join",
                Author = new EmbedAuthorBuilder()
                             .WithName($"Party Leader: {component.User.Username}")
                             .WithIconUrl(Program.Client.GetUser(component.User.Id).GetAvatarUrl()),
                ThumbnailUrl = StaticObjects.cubeIconUrl,
                ImageUrl = eventImages[cubeName],
                Color = Color.Red,
            };

            if (!string.IsNullOrEmpty(customMessage))
            {
                embed.AddField(new EmbedFieldBuilder()
                {
                    Name = "Custom Message",
                    Value = customMessage,
                });
            }

            embed.AddField(new EmbedFieldBuilder().WithName($"{component.User.Username} has joined").WithValue($"{component.User.Mention}").WithIsInline(true));

            await component.UpdateAsync(x =>
            {
                x.Embed = embed.Build();

                x.Components = new ComponentBuilder()
                .WithButton(StaticObjects.joinButton)
                .WithButton(StaticObjects.leaveButton)
                .WithButton(StaticObjects.kickButton)
                .WithButton(StaticObjects.deleteButton)
                .WithButton(StaticObjects.startButton)
                .Build();
            });

            ITextChannel textChannel = (ITextChannel)component.Message.Channel;
            IThreadChannel threadChannel = await textChannel.CreateThreadAsync(name: cubeName, message: component.Message, autoArchiveDuration: ThreadArchiveDuration.OneDay);

            List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));

            ThreadLinkedMessage threadLinkedMessage = new()
            {
                ThreadId = threadChannel.Id,
                MessageId = component.Message.Id
            };

            threadLinkedMessageList.Add(threadLinkedMessage);
            File.WriteAllText("ThreadMessageLink.json", JsonSerializer.Serialize(threadLinkedMessageList));
        }
    }
}