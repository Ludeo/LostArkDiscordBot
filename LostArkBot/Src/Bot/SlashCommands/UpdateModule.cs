﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System;
using LostArkBot.Src.Bot.Shared;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class UpdateModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
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
            characterName = characterName.ToTitleCase();

            Character oldCharacter = characterList.Find(x => x.CharacterName.ToLower() == characterName.ToLower());
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

            if (className is not ClassName.Default)
            {
                newCharacter.ClassName = className.ToString();
            }

            if (itemLevel is not 0)
            {
                newCharacter.ItemLevel = itemLevel;
            }

            if (!string.IsNullOrEmpty(engravings))
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
                foreach (string engraving in splitEngravings)
                {
                    parsedEngravings.Add(engraving.Trim().ToTitleCase());
                }

                newCharacter.Engravings = String.Join(", ", parsedEngravings);
            }

            if (crit is not 0)
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
                ThumbnailUrl = profilePicture == string.Empty ? Context.Guild.GetUser(newCharacter.DiscordUserId).GetAvatarUrl() : profilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", newCharacter.ItemLevel, true);
            embedBuilder.AddField("Class", newCharacter.ClassName, true);

            string engravingsString = "\u200b";
            foreach (string x in newCharacter.Engravings.Split(","))
            {
                engravingsString += x + "\n";
            }

            embedBuilder.AddField("Engravings", engravingsString, true);
            embedBuilder.AddField("Stats", $"Crit: {newCharacter.Crit}\nSpec: {newCharacter.Spec}\nDom: {newCharacter.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {newCharacter.Swift}\nEnd: {newCharacter.End}\nExp: {newCharacter.Exp}", true);
            if (customMessage != string.Empty)
            {
                embedBuilder.AddField("Custom Message", customMessage);
            }

            await RespondAsync(text: $"{characterName} got successfully updated", embed: embedBuilder.Build());
        }
    }
}