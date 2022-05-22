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
            string engravings = command.Data.Options.First(x => x.Name == "engravings").Value.ToString();
            string crit = command.Data.Options.First(x => x.Name == "crit").Value.ToString();
            string spec = command.Data.Options.First(x => x.Name == "spec").Value.ToString();
            string dom = command.Data.Options.First(x => x.Name == "dom").Value.ToString();
            string swift = command.Data.Options.First(x => x.Name == "swift").Value.ToString();
            string end = command.Data.Options.First(x => x.Name == "end").Value.ToString();
            string exp = command.Data.Options.First(x => x.Name == "exp").Value.ToString();

            SocketSlashCommandDataOption profilePictureObject =
                command.Data.Options.FirstOrDefault(x => x.Name == "profile-picture");

            string profilePicture = "";

            if (profilePictureObject is not null)
            {
                profilePicture = profilePictureObject.Value.ToString();
            }

            SocketSlashCommandDataOption profileMessageObject =
                command.Data.Options.FirstOrDefault(x => x.Name == "custom-profile-message");

            string customMessage = "";

            if (profileMessageObject is not null)
            {
                customMessage = profileMessageObject.Value.ToString();
            }

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
            string engraving = string.Empty;

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