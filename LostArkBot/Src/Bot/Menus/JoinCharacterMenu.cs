using Discord;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Menus
{
    internal class JoinCharacterMenu
    {
        public static async Task JoinCharacter(SocketMessageComponent component)
        {
            string messageIdRaw = component.Data.Values.First();
            string messageIdString = messageIdRaw.Split(",")[0];
            ulong messageId = ulong.Parse(messageIdString);
            ITextChannel channel = Program.Client.GetChannel(component.Message.Channel.Id) as ITextChannel;
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

            if (originalEmbed.Timestamp != null)
            {
                newEmbed.Timestamp = originalEmbed.Timestamp.Value;
            }

            bool addedCharacter = false;
            ulong userId = component.User.Id;
            string userMention = component.User.Mention;

            List<Character> characterList =
                JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));

            List<Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            string characterName = string.Empty;

            foreach (EmbedField originalEmbedField in originalEmbed.Fields)
            {
                if (originalEmbedField.Value.Contains(userMention))
                {
                    if (messageIdRaw.Split(",")[1] == "Default")
                    {
                        newEmbed.AddField($"{component.User.Username} has joined", $"{component.User.Mention}\nIGN: \niLvl: \nClass: ", true);

                        addedCharacter = true;
                    }
                    else
                    {
                        Character character = characters.Find(x => x.CharacterName == messageIdRaw.Split(",")[1]);
                        characterName = character.CharacterName;

                        newEmbed.AddField(
                                            $"{component.User.Username} has joined",
                                            $"{component.User.Mention}\nIGN: {character.CharacterName}\niLvl: {character.ItemLevel}\n"
                                        + $"Class: {character.ClassName}",
                                            true);

                        addedCharacter = true;
                    }
                }
                else
                {
                    if (originalEmbedField.Name.Contains("Custom Message"))
                    {
                        newEmbed.AddField(originalEmbedField.Name, originalEmbedField.Value, false);
                    }
                    else
                    {
                        newEmbed.AddField(originalEmbedField.Name, originalEmbedField.Value, true);
                    }
                }
            }

            if (addedCharacter == false)
            {
                string title = originalEmbed.Title;
                string title1 = title.Split("(")[1];
                string title2 = title1.Split(")")[0];
                string playerNumberJoined = title2.Split("/")[0];
                string playerNumberMax = title2.Split("/")[1];

                if (int.Parse(playerNumberJoined) == int.Parse(playerNumberMax))
                {
                    await component.RespondAsync(text: "This lobby is already full", ephemeral: true);

                    return;
                }

                if (messageIdRaw.Split(",")[1] == "Default")
                {
                    newEmbed.AddField($"{component.User.Username} has joined", $"{component.User.Mention}\nIGN: \niLvl: \nClass: ", true);
                }
                else
                {
                    Character character = characters.Find(x => x.CharacterName == messageIdRaw.Split(",")[1]);
                    characterName = character.CharacterName;

                    newEmbed.AddField(
                                        $"{component.User.Username} has joined",
                                        $"{component.User.Mention}\nIGN: {character.CharacterName}\niLvl: {character.ItemLevel}\n"
                                    + $"Class: {character.ClassName}",
                                        true);
                }

                newEmbed.Title = $"{title.Split("(")[0]}({int.Parse(playerNumberJoined) + 1}/{playerNumberMax})";
            }

            await message.ModifyAsync(x =>
            {
                x.Embed = newEmbed.Build();
            });

            List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
            ThreadLinkedMessage linkedMessage = threadLinkedMessageList.First(x => x.MessageId == messageId);

            IThreadChannel threadChannel = Program.Client.GetChannel(linkedMessage.ThreadId) as IThreadChannel;
            await threadChannel.AddUserAsync(component.User as IGuildUser);

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