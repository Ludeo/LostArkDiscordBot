using Discord;
using Discord.Interactions;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class RegisterModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("register", "Registers the given character")]
        public async Task Register(
            [Summary("character-name", "Name of the character you want to register")] string characterName,
            [Summary("class-name", "Class of your character")] ClassName className,
            [Summary("item-level", "Item Level of your character")] int itemLevel,
            [Summary("engravings", "Engravings of your character, seperated by a comma")] string engravings = "",
            [Summary("crit", "How much Crit your character has")] int crit = 0,
            [Summary("spec", "How much Specialization your character has")] int spec = 0,
            [Summary("dom", "How much Domination your character has")] int dom = 0,
            [Summary("swift", "How much Swiftness your character has")] int swift = 0,
            [Summary("end", "How much Endurance your character has")] int end = 0,
            [Summary("exp", "How much Expertise your character has")] int exp = 0,
            [Summary("profile-picture", "Link for a profile picture")] string profilePicture = "",
            [Summary("custom-profile-message", "Custom message for your profile")] string customMessage = "")
        {
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            Character oldCharacter = characterList.Find(x => x.CharacterName == characterName);

            if (oldCharacter is not null)
            {
                await RespondAsync(text: $"{characterName} is already registered. You can update it with **/update**", ephemeral: true);

                return;
            }

            Character newCharacter = new()
            {
                DiscordUserId = Context.User.Id,
                CharacterName = characterName,
                ClassName = className.ToString(),
                ItemLevel = itemLevel,
                Engravings = engravings,
                Crit = crit,
                Spec = spec,
                Dom = dom,
                Swift = swift,
                End = end,
                Exp = exp,
                CustomProfileMessage = customMessage,
                ProfilePicture = profilePicture,
            };

            characterList.Add(newCharacter);
            await File.WriteAllTextAsync("characters.json", JsonSerializer.Serialize(characterList));

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {characterName}",
                ThumbnailUrl = profilePicture == string.Empty ? Program.Client.GetUser(newCharacter.DiscordUserId).GetAvatarUrl() : profilePicture,
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

            await RespondAsync(text: $"{characterName} got successfully registered", embed: embedBuilder.Build());
        }
    }
}