using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LostArkBot.Bot.Modules
{
    public static class RegisterModule
    {
        public static async Task RegisterModuleAsync(SocketSlashCommand command)
        {
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            string characterName = command.Data.Options.First(x => x.Name == "character-name").Value.ToString();

            Character oldCharacter = characterList.Find(x => x.CharacterName == characterName);

            if (oldCharacter is not null)
            {
                await command.RespondAsync(text: $"{characterName} is already registered. You can update it with **/update**", ephemeral: true);

                return;
            }

            string className = command.Data.Options.First(x => x.Name == "class-name").Value.ToString();
            int itemLevel = int.Parse(command.Data.Options.First(x => x.Name == "item-level").Value.ToString()!);

            #region checking for null values
            SocketSlashCommandDataOption engravingsObject = command.Data.Options.FirstOrDefault(x => x.Name == "engravings");
            string engravings = "";

            if(engravingsObject is not null)
            {
                engravings = engravingsObject.Value.ToString();
            }

            SocketSlashCommandDataOption critObject = command.Data.Options.FirstOrDefault(x => x.Name == "crit");
            string crit = "";

            if (critObject is not null)
            {
                crit = critObject.Value.ToString();
            }

            SocketSlashCommandDataOption specObject = command.Data.Options.FirstOrDefault(x => x.Name == "spec");
            string spec = "";

            if (specObject is not null)
            {
                spec = specObject.Value.ToString();
            }

            SocketSlashCommandDataOption domObject = command.Data.Options.FirstOrDefault(x => x.Name == "dom");
            string dom = "";

            if (domObject is not null)
            {
                dom = domObject.Value.ToString();
            }

            SocketSlashCommandDataOption swiftObject = command.Data.Options.FirstOrDefault(x => x.Name == "swift");
            string swift = "";

            if (swiftObject is not null)
            {
                swift = swiftObject.Value.ToString();
            }

            SocketSlashCommandDataOption endObject = command.Data.Options.FirstOrDefault(x => x.Name == "end");
            string end = "";

            if (endObject is not null)
            {
                end = endObject.Value.ToString();
            }

            SocketSlashCommandDataOption expObject = command.Data.Options.FirstOrDefault(x => x.Name == "exp");
            string exp = "";

            if (expObject is not null)
            {
                exp = expObject.Value.ToString();
            }

            SocketSlashCommandDataOption profilePictureObject = command.Data.Options.FirstOrDefault(x => x.Name == "profile-picture");
            string profilePicture = "";

            if (profilePictureObject is not null)
            {
                profilePicture = profilePictureObject.Value.ToString();
            }

            SocketSlashCommandDataOption profileMessageObject = command.Data.Options.FirstOrDefault(x => x.Name == "custom-profile-message");
            string customMessage = "";

            if (profileMessageObject is not null)
            {
                customMessage = profileMessageObject.Value.ToString();
            }
            #endregion

            Character newCharacter = new()
            {
                DiscordUserId = command.User.Id,
                CharacterName = characterName,
                ClassName = className,
                ItemLevel = itemLevel,
                Engravings = engravings,
                Crit = crit,
                Spec = spec,
                Dom = dom,
                Swift = swift,
                End = end,
                Exp = exp,
                ProfilePicture = profilePicture,
                CustomProfileMessage = customMessage,
            };

            characterList.Add(newCharacter);
            await File.WriteAllTextAsync("characters.json", JsonSerializer.Serialize(characterList));

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {characterName}",
                ThumbnailUrl = profilePicture == string.Empty
                    ? Program.Client.GetUser(newCharacter.DiscordUserId).GetAvatarUrl()
                    : profilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", itemLevel, true);
            embedBuilder.AddField("Class", className, true);

            string[] engravings2 = engravings.Split(",");
            string engraving = "\u200b";

            foreach (string x in engravings2)
            {
                engraving += x + "\n";
            }

            embedBuilder.AddField("Engravings", engraving, true);
            embedBuilder.AddField("Stats", $"Crit: {crit}\nSpec: {spec}\nDom: {dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {swift}\nEnd: {end}\nExp: {exp}", true);
            embedBuilder.AddField("Custom Message", customMessage == string.Empty ? "\u200b" : customMessage);

            await command.RespondAsync(text: $"{characterName} got successfully registered", embed: embedBuilder.Build());
        }
    }
}