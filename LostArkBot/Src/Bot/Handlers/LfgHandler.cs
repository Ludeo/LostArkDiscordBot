using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    public class LfgHandler
    {
        public static async Task LfgHandlerAsync(SocketMessageComponent component, LfgModel model)
        {
            string customMessage = component.Message.Embeds.First().Footer?.Text;
            ComponentBuilder componentBuilder = new();

            Console.WriteLine(model.IsEnd);

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

                embed.Title = $"{model.Title} {component.Data.Values.First()} (0/{model.Players})";
                embed.Author = new EmbedAuthorBuilder()
                             .WithName($"Party Leader: {user.DisplayName}")
                             .WithIconUrl(user.GetAvatarUrl());
                embed.Description = "Waiting for players to join...";
                embed.ImageUrl = Program.StaticObjects.EventImages[component.Data.Values.First()];

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

                componentBuilder.WithButton(Program.StaticObjects.JoinButton)
                .WithButton(Program.StaticObjects.LeaveButton)
                .WithButton(Program.StaticObjects.KickButton)
                .WithButton(Program.StaticObjects.DeleteButton)
                .WithButton(Program.StaticObjects.StartButton);

                ITextChannel textChannel = (ITextChannel)component.Message.Channel;
                await textChannel.CreateThreadAsync(name: component.Data.Values.First(), message: component.Message, autoArchiveDuration: ThreadArchiveDuration.OneDay);

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

                componentBuilder.WithSelectMenu(menuBuilder).WithButton(Program.StaticObjects.HomeButton).WithButton(Program.StaticObjects.DeleteButton);
            }

            await component.UpdateAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = componentBuilder.Build();
            });
        }
    }
}