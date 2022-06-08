using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    public class LfgHandler
    {
        public static async Task LfgHandlerAsync(SocketMessageComponent component, LfgModel model, Dictionary<string, string> eventImages)
        {
            string customMessage = component.Message.Embeds.First().Footer?.Text;
            ComponentBuilder componentBuilder = new();

            EmbedBuilder embed = new()
            {
                Title = model.Title,
                Description = model.Description,
                ThumbnailUrl = model.ThumbnailUrl,
                Color = model.Color,
            };

            if(model.IsEnd)
            {
                SocketGuildUser user = await component.Channel.GetUserAsync(component.User.Id) as SocketGuildUser;

                embed.Author = new EmbedAuthorBuilder()
                             .WithName($"Party Leader: {user.DisplayName}")
                             .WithIconUrl(user.GetAvatarUrl());
                embed.Description = "Waiting for players to join...";
                embed.ImageUrl = eventImages[model.MenuItemId];

                if (component.Message.Embeds.First().Timestamp != null)
                {
                    embed.AddField(new EmbedFieldBuilder()
                    {
                        Name = "Time",
                        Value = $"<t:{component.Message.Embeds.First().Timestamp.Value.ToUnixTimeSeconds()}:F>"
                    });
                }

                if (!string.IsNullOrEmpty(customMessage))
                {
                    embed.AddField(new EmbedFieldBuilder()
                    {
                        Name = "Custom Message",
                        Value = customMessage,
                    });
                }

                componentBuilder.WithButton(StaticObjects.joinButton)
                .WithButton(StaticObjects.leaveButton)
                .WithButton(StaticObjects.kickButton)
                .WithButton(StaticObjects.deleteButton)
                .WithButton(StaticObjects.startButton);

                ITextChannel textChannel = (ITextChannel)component.Message.Channel;
                IThreadChannel threadChannel = await textChannel.CreateThreadAsync(name: component.Data.Values.First(), message: component.Message, autoArchiveDuration: ThreadArchiveDuration.OneDay);

                List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));

                ThreadLinkedMessage threadLinkedMessage = new()
                {
                    ThreadId = threadChannel.Id,
                    MessageId = component.Message.Id,
                    ChannelId = component.Channel.Id,
                };

                threadLinkedMessageList.Add(threadLinkedMessage);
                File.WriteAllText("ThreadMessageLink.json", JsonSerializer.Serialize(threadLinkedMessageList));

            } else
            {
                if (component.Message.Embeds.First().Timestamp != null)
                {
                    embed.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                }

                if (!string.IsNullOrEmpty(customMessage))
                {
                    embed.Footer = new EmbedFooterBuilder() {
                        Text = customMessage,
                    };
                }

                SelectMenuBuilder menuBuilder = new SelectMenuBuilder().WithCustomId(model.MenuItemId).WithPlaceholder(model.MenuPlaceholder);

                foreach(MenuBuilderOption option in model.MenuBuilderOptions)
                {
                    menuBuilder.AddOption(new SelectMenuOptionBuilder().WithLabel(option.Label).WithValue(option.Value).WithDescription(option.Description));
                }

                componentBuilder.WithSelectMenu(menuBuilder).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton);
            }

            await component.UpdateAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = componentBuilder.Build();
            });
        }
    }
}