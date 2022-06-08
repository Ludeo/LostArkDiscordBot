using Discord;
using Discord.Net;
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
    public class ManageUserHandler
    {
        public static async Task ManageUserHandlerAsync(SocketMessageComponent component, ManageUserModel model)
        {
            string characterName = component.Data.Values.First();
            ulong messageId = (ulong)component.Message.Reference.MessageId;
            ITextChannel channel = component.Channel as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(messageId);
            IUserMessage message = messageRaw as IUserMessage;
            Embed originalEmbed = messageRaw.Embeds.First() as Embed;

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

            bool characterAdded = false;

            string title = originalEmbed.Title;
            string title1 = title.Split("(")[1];
            string title2 = title1.Split(")")[0];
            int playerNumberJoined = int.Parse(title2.Split("/")[0]);
            string playerNumberMax = title2.Split("/")[1];

            SocketGuildUser user = await component.Channel.GetUserAsync(component.User.Id) as SocketGuildUser;
            IThreadChannel threadChannel = user.Guild.GetChannel(messageId) as IThreadChannel;
            
            List<Character> characterList =
                JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));

            foreach (EmbedField field in originalEmbed.Fields)
            {
                if (field.Name == "Custom Message" || field.Name == "Time")
                {
                    newEmbed.AddField(new EmbedFieldBuilder().WithName(field.Name).WithValue(field.Value).WithIsInline(field.Inline));
                    continue;
                }

                if(model.Action == ManageAction.Join)
                {
                    if (field.Value.Contains(component.User.Mention))
                    {
                        if (component.Data.Values.First() == "Default")
                        {
                            newEmbed.AddField(field.Name, $"{component.User.Mention}", true);

                            characterAdded = true;
                        }
                        else
                        {
                            Character character = characterList.Find(x => x.CharacterName == characterName);

                            List<GuildEmote> emotes = Program.GuildEmotes;
                            GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

                            newEmbed.AddField(
                                                field.Name,
                                                $"{component.User.Mention}\n{character.CharacterName}\n{character.ItemLevel}\n"
                                                + $"<:{emote.Name}:{emote.Id}> {character.ClassName}",
                                                true);

                            characterAdded = true;
                        }
                    }
                } else if(model.Action == ManageAction.Kick)
                {
                    if (field.Value.Split("\n")[1] == characterName)
                    {
                        string mention = field.Value.Split("\n")[0];
                        ulong discordId = ulong.Parse(mention.Replace("<", "").Replace(">", "").Replace("!", "").Replace("@", ""));
                        await threadChannel.RemoveUserAsync(user);
                        playerNumberJoined--;
                    } else
                    {
                        newEmbed.AddField(new EmbedFieldBuilder().WithName(field.Name).WithValue(field.Value).WithIsInline(field.Inline));
                    }
                }
            }

            if (characterAdded == false && model.Action == ManageAction.Join)
            {
                if (playerNumberJoined == int.Parse(playerNumberMax))
                {
                    await component.RespondAsync(text: "This lobby is already full", ephemeral: true);

                    return;
                }

                if (characterName == "Default")
                {
                    newEmbed.AddField($"{user.DisplayName} has joined", $"{component.User.Mention}", true);
                }
                else
                {
                    Character character = characterList.Find(x => x.CharacterName == characterName);

                    List<GuildEmote> emotes = Program.GuildEmotes;
                    GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

                    newEmbed.AddField(
                                        $"{user.DisplayName} has joined",
                                        $"{component.User.Mention}\n{character.CharacterName}\n{character.ItemLevel}\n"
                                            + $"<:{emote.Name}:{emote.Id}> {character.ClassName}",
                                        true);
                }

                playerNumberJoined++;
                await threadChannel.AddUserAsync(user);
            }

            newEmbed.Title = $"{title.Split("(")[0]}({playerNumberJoined}/{playerNumberMax})";

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
                await Program.Log(new LogMessage(LogSeverity.Error, "JoinCharacterMenu.cs", exception.Message));
            }
        }
    }
}