using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class RegisterModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
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
            characterName = characterName.ToTitleCase();
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            Character oldCharacter = characterList.Find(x => x.CharacterName.ToLower() == characterName.ToLower());

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

            if (!string.IsNullOrEmpty(newCharacter.Engravings))
            {
                List<string> splitEngravings = new();
                if (engravings.Contains(","))
                {
                    splitEngravings = (engravings.Split(",")).ToList();
                }
                else if (engravings.Contains("\\"))
                {
                    splitEngravings = engravings.Split("\\").ToList();
                }
                else if (engravings.Contains("/"))
                {
                    splitEngravings = engravings.Split("/").ToList();
                }
                else
                {
                    MatchCollection matches = Regex.Matches(engravings, "([[a-zA-Z\\s]+\\d)");
                    foreach (Match match in matches)
                    {
                        splitEngravings.Add(match.ToString());
                    }
                }

                List<string> parsedEngravings = new();
                foreach (string eng in splitEngravings)
                {
                    parsedEngravings.Add(eng.Trim().ToTitleCase());
                }

                newCharacter.Engravings = String.Join(", ", parsedEngravings);
            }

            characterList.Add(newCharacter);
            await File.WriteAllTextAsync("characters.json", JsonSerializer.Serialize(characterList));

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {characterName}",
                ThumbnailUrl = profilePicture == string.Empty ? Context.Guild.GetUser(newCharacter.DiscordUserId).GetAvatarUrl() : profilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", itemLevel, true);
            embedBuilder.AddField("Class", className, true);

            string engravingsString = "\u200b";
            foreach (string x in engravings.Split(","))
            {
                engravingsString += x + "\n";
            }

            embedBuilder.AddField("Engravings", engravingsString, true);
            embedBuilder.AddField("Stats", $"Crit: {crit}\nSpec: {spec}\nDom: {dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {swift}\nEnd: {end}\nExp: {exp}", true);
            if (customMessage != string.Empty)
            {
                embedBuilder.AddField("Custom Message", customMessage);
            }

            await RespondAsync(text: $"{characterName} got successfully registered", embed: embedBuilder.Build());
        }
    }
}
