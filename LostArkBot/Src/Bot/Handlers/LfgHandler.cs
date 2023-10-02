using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Bot.Models;
using LostArkBot.databasemodels;
using Microsoft.EntityFrameworkCore;

namespace LostArkBot.Bot.Handlers;

public static class LfgHandler
{
    public static async Task LfgHandlerAsync(SocketMessageComponent component, LfgModel model, LostArkBotContext dbcontext)
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
            SocketGuildUser guildUser = await component.Channel.GetUserAsync(component.User.Id) as SocketGuildUser;

            embed.Title = $"{model.Title} {component.Data.Values.First()} (0/{model.Players})";

            embed.Author = new EmbedAuthorBuilder()
                           .WithName($"Party Leader: {guildUser.DisplayName}\n{guildUser.Id}")
                           .WithIconUrl(guildUser.GetAvatarUrl());

            embed.Description = "Waiting for players to join...";
            embed.ImageUrl = Program.StaticObjects.EventImages[component.Data.Values.First()];

            if (component.Message.Embeds.First().Timestamp != null)
            {
                embed.AddField(
                               new EmbedFieldBuilder
                               {
                                   Name = "Time",
                                   Value = $"<t:{component.Message.Embeds.First().Timestamp!.Value.ToUnixTimeSeconds()}:F>\n"
                                         + $"<t:{component.Message.Embeds.First().Timestamp!.Value.ToUnixTimeSeconds()}:R>",
                               });
            }

            if (!string.IsNullOrEmpty(customMessage))
            {
                embed.AddField(
                               new EmbedFieldBuilder
                               {
                                   Name = "Custom Message",
                                   Value = customMessage,
                               });
            }

            string staticGroupName = component.Message.CleanContent;
            ITextChannel textChannel = (ITextChannel)component.Message.Channel;

            if (!string.IsNullOrEmpty(staticGroupName))
            {
                StaticGroup staticGroup = dbcontext.StaticGroups.Include(x => x.Characters)
                                                   .ThenInclude(x => x.User).FirstOrDefault(x => x.Name == staticGroupName);

                List<GuildEmote> emotes = Program.GuildEmotes;

                foreach (Character character in staticGroup.Characters)
                {
                    IGuildUser playerUser = await textChannel.Guild.GetUserAsync(character.User.DiscordUserId);
                    GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

                    embed.AddField(
                                   playerUser.DisplayName + " has joined",
                                   $"{playerUser.Mention}\n{character.CharacterName}\n{character.ItemLevel}\n"
                                 + $"<:{emote.Name}:{emote.Id}> {character.ClassName}",
                                   true);
                }

                embed.Title = $"{model.Title} {component.Data.Values.First()} ({staticGroup.Characters.Count}/{model.Players})";
            }

            componentBuilder.WithButton(Program.StaticObjects.JoinButton)
                            .WithButton(Program.StaticObjects.LeaveButton)
                            .WithButton(Program.StaticObjects.KickButton)
                            .WithButton(Program.StaticObjects.DeleteButton)
                            .WithButton(Program.StaticObjects.StartButton);

            await component.ModifyOriginalResponseAsync(
                                                        x =>
                                                        {
                                                            x.Content = "LFG successfully created";
                                                            x.Components = new ComponentBuilder().Build();
                                                            x.Embeds = null;
                                                        });

            IMessage endMessage = await component.FollowupAsync(embed: embed.Build(), components: componentBuilder.Build());

            IThreadChannel threadChannel = await textChannel.CreateThreadAsync(component.Data.Values.First(), message: endMessage, type: ThreadType.PrivateThread);
            await threadChannel.ModifyAsync(x => x.AutoArchiveDuration = ThreadArchiveDuration.OneWeek);
            await threadChannel.SendMessageAsync("\uFEFF \uFEFF ", components: componentBuilder.Build());
        }
        else
        {
            if (component.Message.Embeds.First().Timestamp != null)
            {
                embed.Timestamp = component.Message.Embeds.First().Timestamp!.Value;
            }

            if (!string.IsNullOrEmpty(customMessage))
            {
                embed.Footer = new EmbedFooterBuilder
                {
                    Text = customMessage,
                };
            }

            SelectMenuBuilder menuBuilder = new SelectMenuBuilder().WithCustomId(model.MenuItemId).WithPlaceholder(model.MenuPlaceholder);

            foreach (MenuBuilderOption option in model.MenuBuilderOptions)
            {
                menuBuilder.AddOption(
                                      new SelectMenuOptionBuilder().WithLabel(option.Label).WithValue(option.Value)
                                                                   .WithDescription(option.Description));
            }

            componentBuilder.WithSelectMenu(menuBuilder).WithButton(Program.StaticObjects.HomeButton);

            await component.ModifyOriginalResponseAsync(
                                                        x =>
                                                        {
                                                            x.Embed = embed.Build();
                                                            x.Components = componentBuilder.Build();
                                                            x.Content = model.IsEnd ? string.Empty : component.Message.Content;
                                                        });
        }
    }
}