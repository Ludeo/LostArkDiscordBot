using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LostArkBot.Bot.Modules
{
    public static class UpdateModule
    {
        public static async Task UpdateModuleAsync(SocketSlashCommand command)
        {
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            ulong discordUserId = command.User.Id;
            string characterName = command.Data.Options.First(x => x.Name == "character-name").Value.ToString();

            Character oldCharacter = characterList.Find(x => x.CharacterName == characterName);
            Character newCharacter = oldCharacter;

            if (oldCharacter is not null)
            {
                if (oldCharacter.DiscordUserId != discordUserId)
                {
                    await command.RespondAsync(text: $"You don't have permissions to update {characterName}", ephemeral: true);

                    return;
                }
            }
            else
            {
                await command.RespondAsync(text: $"{characterName} is not registered. You can register it with **/register**", ephemeral: true);

                return;
            }

            SocketSlashCommandDataOption classNameObject = command.Data.Options.FirstOrDefault(x => x.Name == "class-name");

            if (classNameObject is not null)
            {
                newCharacter.ClassName = classNameObject.Value.ToString();
            }

            SocketSlashCommandDataOption itemLevelObject = command.Data.Options.FirstOrDefault(x => x.Name == "item-level");

            if (itemLevelObject is not null)
            {
                newCharacter.ItemLevel = int.Parse(itemLevelObject.Value.ToString()!);
            }

            SocketSlashCommandDataOption engravingsObject = command.Data.Options.FirstOrDefault(x => x.Name == "engravings");

            if (engravingsObject is not null)
            {
                newCharacter.Engravings = engravingsObject.Value.ToString();
            }

            SocketSlashCommandDataOption critObject = command.Data.Options.FirstOrDefault(x => x.Name == "crit");

            if (critObject is not null)
            {
                newCharacter.Crit = critObject.Value.ToString();
            }

            SocketSlashCommandDataOption specObject = command.Data.Options.FirstOrDefault(x => x.Name == "spec");

            if (specObject is not null)
            {
                newCharacter.Spec = specObject.Value.ToString();
            }

            SocketSlashCommandDataOption domObject = command.Data.Options.FirstOrDefault(x => x.Name == "dom");

            if (domObject is not null)
            {
                newCharacter.Dom = domObject.Value.ToString();
            }

            SocketSlashCommandDataOption swiftObject = command.Data.Options.FirstOrDefault(x => x.Name == "swift");

            if (swiftObject is not null)
            {
                newCharacter.Swift = swiftObject.Value.ToString();
            }

            SocketSlashCommandDataOption endObject = command.Data.Options.FirstOrDefault(x => x.Name == "end");

            if (endObject is not null)
            {
                newCharacter.End = endObject.Value.ToString();
            }

            SocketSlashCommandDataOption expObject = command.Data.Options.FirstOrDefault(x => x.Name == "exp");

            if (expObject is not null)
            {
                newCharacter.Exp = expObject.Value.ToString();
            }

            SocketSlashCommandDataOption profilePictureObject =
                command.Data.Options.FirstOrDefault(x => x.Name == "profile-picture");

            if (profilePictureObject is not null)
            {
                newCharacter.ProfilePicture = profilePictureObject.Value.ToString();
            }

            SocketSlashCommandDataOption profileMessageObject =
                command.Data.Options.FirstOrDefault(x => x.Name == "custom-profile-message");

            if (profileMessageObject is not null)
            {
                newCharacter.CustomProfileMessage = profileMessageObject.Value.ToString();
            }

            characterList.Add(newCharacter);
            characterList.Remove(oldCharacter);
            await File.WriteAllTextAsync("characters.json", JsonSerializer.Serialize(characterList));

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {characterName}",
                ThumbnailUrl = newCharacter.ProfilePicture == string.Empty
                    ? Program.Client.GetUser(newCharacter.DiscordUserId).GetAvatarUrl()
                    : newCharacter.ProfilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", newCharacter.ItemLevel, true);
            embedBuilder.AddField("Class", newCharacter.ClassName, true);

            string[] engravings = newCharacter.Engravings.Split(",");
            string engraving = string.Empty;

            foreach (string x in engravings)
            {
                engraving += x + "\n";
            }

            embedBuilder.AddField("Engravings", engraving, true);
            embedBuilder.AddField("Stats", $"Crit: {newCharacter.Crit}\nSpec: {newCharacter.Spec}\nDom: {newCharacter.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {newCharacter.Swift}\nEnd: {newCharacter.End}\nExp: {newCharacter.Exp}", true);
            embedBuilder.AddField("Custom Message", newCharacter.CustomProfileMessage == string.Empty ? "\u200b" : newCharacter.CustomProfileMessage);

            await command.RespondAsync(text: $"{characterName} got successfully updated", embed: embedBuilder.Build(), ephemeral: true);
        }
    }
}