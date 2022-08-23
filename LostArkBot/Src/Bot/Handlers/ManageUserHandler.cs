using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Bot.Models;
using LostArkBot.Bot.Shared;
using LostArkBot.databasemodels;

namespace LostArkBot.Bot.Handlers;

public static class ManageUserHandler
{
    private static readonly List<GuildEmote> Emotes = Program.GuildEmotes;

    private static EmbedFieldBuilder CopyField(EmbedField field) =>
        new EmbedFieldBuilder().WithName(field.Name).WithValue(field.Value).WithIsInline(field.Inline);

    private static async Task<EmbedFieldBuilder> GetCharacterFieldAsync(Character character, SocketInteraction component)
    {
        SocketGuildUser interactionUser;

        if (component.Channel.GetChannelType() == ChannelType.PublicThread)
        {
            ITextChannel channel = ((SocketThreadChannel)component.Channel).ParentChannel as ITextChannel;
            interactionUser = await channel.GetUserAsync(component.User.Id) as SocketGuildUser;
        }
        else
        {
            interactionUser = await component.Channel.GetUserAsync(component.User.Id) as SocketGuildUser;
        }

        EmbedFieldBuilder embedField = new();

        if (character != null)
        {
            GuildEmote emote = Emotes.Find(x => x.Name == character.ClassName.ToLower());
            embedField.WithName(interactionUser.DisplayName);

            embedField.WithValue(
                                 $"{component.User.Mention}\n"
                               + $"{character.CharacterName}\n"
                               + $"{character.ItemLevel}\n"
                               + $"<:{emote.Name}:{emote.Id}> {character.ClassName}");

            embedField.WithIsInline(true);
        }
        else
        {
            embedField.WithName($"{interactionUser.DisplayName}");
            embedField.WithValue($"{component.User.Mention}\n" + "No character");
            embedField.WithIsInline(true);
        }

        return embedField;
    }

    public static async Task ManageUserHandlerAsync(SocketMessageComponent component, ManageUserModel model, LostArkBotContext dbcontext)
    {
        string characterName = component.Data.Values.First();
        SocketThreadChannel threadChannel;
        ITextChannel channel;
        Embed originalEmbed;
        SocketGuildUser interactionUser;
        IUserMessage message;

        if (component.Channel.GetChannelType() == ChannelType.PublicThread)
        {
            threadChannel = component.Channel as SocketThreadChannel;
            channel = threadChannel.ParentChannel as ITextChannel;
            message = await channel.GetMessageAsync(threadChannel.Id) as IUserMessage;
            originalEmbed = message.Embeds.First() as Embed;
            interactionUser = await channel.GetUserAsync(component.User.Id) as SocketGuildUser;
        }
        else
        {
            interactionUser = await component.Channel.GetUserAsync(component.User.Id) as SocketGuildUser;
            threadChannel = interactionUser.Guild.GetChannel((ulong)component.Message.Reference.MessageId) as SocketThreadChannel;
            channel = component.Channel as ITextChannel;
            message = await channel.GetMessageAsync((ulong)component.Message.Reference.MessageId) as IUserMessage;
            originalEmbed = message.Embeds.First() as Embed;
        }

        EmbedBuilder newEmbed = new()
        {
            Title = originalEmbed.Title,
            Description = originalEmbed.Description,
            Author = new EmbedAuthorBuilder
            {
                Name = originalEmbed.Author!.Value.Name,
                IconUrl = originalEmbed.Author!.Value.IconUrl,
            },
            Color = originalEmbed.Color!.Value,
        };

        if (originalEmbed.Thumbnail != null)
        {
            newEmbed.ThumbnailUrl = originalEmbed.Thumbnail.Value.Url;
        }

        if (originalEmbed.Image != null)
        {
            newEmbed.ImageUrl = originalEmbed.Image.Value.Url;
        }

        bool characterAdded = false;

        (string title, string playerCounter) = originalEmbed.Title.Split('(', ')');
        (int playerNumberJoined, int playerNumberMax) = Array.ConvertAll(playerCounter.Split("/"), int.Parse);

        EmbedField msgField = originalEmbed.Fields.FirstOrDefault(field => field.Name == "Custom Message");

        if (msgField.Name != null)
        {
            newEmbed.AddField(new EmbedFieldBuilder().WithName(msgField.Name).WithValue(msgField.Value).WithIsInline(msgField.Inline));
        }

        EmbedField timeField = originalEmbed.Fields.FirstOrDefault(field => field.Name == "Time");

        if (timeField.Name != null)
        {
            newEmbed.AddField(new EmbedFieldBuilder().WithName(timeField.Name).WithValue(timeField.Value).WithIsInline(timeField.Inline));
        }

        IEnumerable<EmbedField> embedFields = originalEmbed.Fields.Where(field => field.Name != "Custom Message" && field.Name != "Time");

        switch (model.Action)
        {
            case ManageAction.Join:
            {
                foreach (EmbedField field in embedFields)
                {
                    if (field.Value.Contains(component.User.Mention))
                    {
                        if (component.Data.Values.First() == "Default")
                        {
                            newEmbed.AddField(await GetCharacterFieldAsync(null, component));
                            characterAdded = true;
                        }
                        else
                        {
                            Character character =
                                dbcontext.Characters.FirstOrDefault(
                                                                    x => string.Equals(
                                                                                       x.CharacterName,
                                                                                       characterName,
                                                                                       StringComparison.CurrentCultureIgnoreCase));

                            newEmbed.AddField(await GetCharacterFieldAsync(character, component));
                            characterAdded = true;
                        }

                        await component.ModifyOriginalResponseAsync(
                                                                    msg =>
                                                                    {
                                                                        msg.Content =
                                                                            $"You changed character for **{title.Trim()}** event to {characterName}";

                                                                        msg.Components = new ComponentBuilder().Build();
                                                                    });
                    }
                    else
                    {
                        newEmbed.AddField(CopyField(field));
                    }
                }

                if (characterAdded == false)
                {
                    if (playerNumberJoined == playerNumberMax)
                    {
                        await component.ModifyOriginalResponseAsync(
                                                                    msg =>
                                                                    {
                                                                        msg.Content = "This lobby is already full";
                                                                        msg.Components = new ComponentBuilder().Build();
                                                                    });

                        return;
                    }

                    if (characterName == "Default")
                    {
                        newEmbed.AddField(await GetCharacterFieldAsync(null, component));
                    }
                    else
                    {
                        Character character = dbcontext.Characters.FirstOrDefault(x => x.CharacterName.ToLower() == characterName.ToLower());
                        newEmbed.AddField(await GetCharacterFieldAsync(character, component));
                    }

                    playerNumberJoined++;
                    await threadChannel.AddUserAsync(interactionUser);

                    await component.ModifyOriginalResponseAsync(
                                                                msg =>
                                                                {
                                                                    msg.Content = $"You joined the **{title.Trim()}** event with {characterName}";
                                                                    msg.Components = new ComponentBuilder().Build();
                                                                });
                }

                break;
            }
            case ManageAction.Kick:
            {
                foreach (EmbedField field in embedFields)
                {
                    if (field.Name.ToLower().Trim() == characterName.ToLower())
                    {
                        string mention = field.Value.Split("\n")[0];
                        ulong discordId = ulong.Parse(mention.Replace("<", "").Replace(">", "").Replace("!", "").Replace("@", ""));
                        IGuildUser userToKick = (IGuildUser)await component.Channel.GetUserAsync(discordId);
                        await threadChannel.RemoveUserAsync(userToKick);
                        playerNumberJoined--;

                        await component.ModifyOriginalResponseAsync(
                                                                    msg =>
                                                                    {
                                                                        msg.Content = $"Successfully kicked {characterName}";
                                                                        msg.Components = new ComponentBuilder().Build();
                                                                    });

                        break;
                    }

                    newEmbed.AddField(new EmbedFieldBuilder().WithName(field.Name).WithValue(field.Value).WithIsInline(field.Inline));
                }

                break;
            }
            default: throw new ArgumentOutOfRangeException();
        }

        newEmbed.Title = $"{title.Trim()} ({playerNumberJoined}/{playerNumberMax})";

        await message.ModifyAsync(x => x.Embed = newEmbed.Build());
    }
}