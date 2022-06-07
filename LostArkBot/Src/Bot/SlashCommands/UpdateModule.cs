using Discord;
using Discord.Interactions;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class UpdateModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("update", "Updates the given character")]
        public async Task Update(
            [Summary("character-name", "Name of the character you want to register")] string characterName,
            [Summary("class-name", "Class of your character")] ClassName className = ClassName.Default,
            [Summary("item-level", "Item Level of your character")] int itemLevel = 0,
            [Summary("engravings", "Engravings of your character, seperated by a comma")] string engravings = null,
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
            ulong discordUserId = Context.User.Id;

            Character oldCharacter = characterList.Find(x => x.CharacterName == characterName);
            Character newCharacter = oldCharacter;

            if (oldCharacter is not null)
            {
                if (oldCharacter.DiscordUserId != discordUserId)
                {
                    await RespondAsync(text: $"You don't have permissions to update {characterName}", ephemeral: true);

                    return;
                }
            }
            else
            {
                await RespondAsync(text: $"{characterName} is not registered. You can register it with **/register**", ephemeral: true);

                return;
            }

            if(className is not ClassName.Default)
            {
                newCharacter.ClassName = className.ToString();
            }

            if(itemLevel is not 0)
            {
                newCharacter.ItemLevel = itemLevel;
            }

            if (!string.IsNullOrEmpty(engravings))
            {
                newCharacter.Engravings = engravings;
            }

            if(crit is not 0)
            {
                newCharacter.Crit = crit;
            }

            if (spec is not 0)
            {
                newCharacter.Spec = spec;
            }

            if (dom is not 0)
            {
                newCharacter.Dom = dom;
            }

            if (swift is not 0)
            {
                newCharacter.Swift = swift;
            }

            if (end is not 0)
            {
                newCharacter.End = end;
            }

            if (exp is not 0)
            {
                newCharacter.Exp = exp;
            }

            if (!string.IsNullOrEmpty(profilePicture))
            {
                newCharacter.ProfilePicture = profilePicture;
            }

            if (!string.IsNullOrEmpty(customMessage))
            {
                newCharacter.CustomProfileMessage = customMessage;
            }

            characterList.Add(newCharacter);
            characterList.Remove(oldCharacter);
            await File.WriteAllTextAsync("characters.json", JsonSerializer.Serialize(characterList));

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {characterName}",
                ThumbnailUrl = profilePicture == string.Empty ? Program.Client.GetUser(newCharacter.DiscordUserId).GetAvatarUrl() : profilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", newCharacter.ItemLevel, true);
            embedBuilder.AddField("Class", newCharacter.ClassName, true);

            string[] engravings2 = newCharacter.Engravings.Split(",");
            string engraving = "\u200b";

            foreach (string x in engravings2)
            {
                engraving += x + "\n";
            }

            embedBuilder.AddField("Engravings", engraving, true);
            embedBuilder.AddField("Stats", $"Crit: {newCharacter.Crit}\nSpec: {newCharacter.Spec}\nDom: {newCharacter.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {newCharacter.Swift}\nEnd: {newCharacter.End}\nExp: {newCharacter.Exp}", true);
            embedBuilder.AddField("Custom Message", customMessage == string.Empty ? "\u200b" : customMessage);

            await RespondAsync(text: $"{characterName} got successfully updated", embed: embedBuilder.Build());
        }
    }
}