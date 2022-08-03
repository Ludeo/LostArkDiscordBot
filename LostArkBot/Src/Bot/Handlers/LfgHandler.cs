using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Models;
using LostArkBot.Src.Bot.Shared;
using System.Collections.Generic;
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

            EmbedBuilder embed = new()
            {
                Title = model.Title,
                Description = model.Description,
                ThumbnailUrl = model.ThumbnailUrl,
                Color = model.Color,
            };

            if (model.IsEnd)
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
                        Value = $"<t:{component.Message.Embeds.First().Timestamp.Value.ToUnixTimeSeconds()}:F>\n" +
                                $"<t:{component.Message.Embeds.First().Timestamp.Value.ToUnixTimeSeconds()}:R>"
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

                string staticGroupName = component.Message.CleanContent;
                ITextChannel textChannel = (ITextChannel)component.Message.Channel;

                if (!string.IsNullOrEmpty(staticGroupName))
                {
                    List<StaticGroup> staticGroups = await JsonParsers.GetStaticGroupsFromJsonAsync();
                    StaticGroup staticGroup = staticGroups.Find(x => x.Name == staticGroupName);
                    List<Character> characters = await JsonParsers.GetCharactersFromJsonAsync();
                    List<GuildEmote> emotes = Program.GuildEmotes;

                    foreach (string player in staticGroup.Players)
                    {
                        Character character = characters.Find(x => x.CharacterName == player);
                        IGuildUser playerUser = await textChannel.Guild.GetUserAsync(character.DiscordUserId);
                        GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

                        embed.AddField(playerUser.DisplayName + " has joined",
                                                $"{playerUser.Mention}\n{character.CharacterName}\n{character.ItemLevel}\n"
                                                + $"<:{emote.Name}:{emote.Id}> {character.ClassName}",
                                                true);
                    }

                    embed.Title = $"{model.Title} {component.Data.Values.First()} ({staticGroup.Players.Count}/{model.Players})";
                }

                componentBuilder.WithButton(Program.StaticObjects.JoinButton)
                .WithButton(Program.StaticObjects.LeaveButton)
                .WithButton(Program.StaticObjects.KickButton)
                .WithButton(Program.StaticObjects.DeleteButton)
                .WithButton(Program.StaticObjects.StartButton);

                IThreadChannel threadChannel = await textChannel.CreateThreadAsync(name: component.Data.Values.First(), message: component.Message);
                await threadChannel.ModifyAsync(x => x.AutoArchiveDuration = ThreadArchiveDuration.OneWeek);

                ComponentBuilder components = new ComponentBuilder().WithButton(Program.StaticObjects.JoinButton)
                                                                .WithButton(Program.StaticObjects.LeaveButton)
                                                                .WithButton(Program.StaticObjects.KickButton)
                                                                .WithButton(Program.StaticObjects.DeleteButton)
                                                                .WithButton(Program.StaticObjects.StartButton);

                await threadChannel.SendMessageAsync(text: "\uFEFF \uFEFF ", components: components.Build());
            }
            else
            {
                if (component.Message.Embeds.First().Timestamp != null)
                {
                    embed.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                }

                if (!string.IsNullOrEmpty(customMessage))
                {
                    embed.Footer = new EmbedFooterBuilder()
                    {
                        Text = customMessage,
                    };
                }

                SelectMenuBuilder menuBuilder = new SelectMenuBuilder().WithCustomId(model.MenuItemId).WithPlaceholder(model.MenuPlaceholder);

                foreach (MenuBuilderOption option in model.MenuBuilderOptions)
                {
                    menuBuilder.AddOption(new SelectMenuOptionBuilder().WithLabel(option.Label).WithValue(option.Value).WithDescription(option.Description));
                }

                componentBuilder.WithSelectMenu(menuBuilder).WithButton(Program.StaticObjects.HomeButton).WithButton(Program.StaticObjects.DeleteButton);
            }

            await component.UpdateAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = componentBuilder.Build();
                x.Content = model.IsEnd ? string.Empty : component.Message.Content;
            });
        }
    }
}