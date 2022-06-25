﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class ProfileModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("profile", "Shows the profile of the given character")]
        public async Task Account([Summary("character-name", "Name of the character")] string characterName)
        {
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            Character character = characterList.Find(x => x.CharacterName == characterName);

            if (character is null)
            {
                await RespondAsync(text: $"{characterName} is not registered. You can register a character with **/register**", ephemeral: true);

                return;
            }

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {characterName}",
                ThumbnailUrl = character.ProfilePicture == string.Empty
                    ? Context.Guild.GetUser(character.DiscordUserId).GetAvatarUrl()
                    : character.ProfilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", character.ItemLevel, true);
            embedBuilder.AddField("Class", character.ClassName, true);

            string[] engravings = character.Engravings.Split(",");
            string engraving = "\u200b";

            foreach (string x in engravings)
            {
                engraving += x + "\n";
            }

            embedBuilder.AddField("Engravings", engraving, true);
            embedBuilder.AddField("Stats", $"Crit: {character.Crit}\nSpec: {character.Spec}\nDom: {character.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {character.Swift}\nEnd: {character.End}\nExp: {character.Exp}", true);
            embedBuilder.AddField("Custom Message", character.CustomProfileMessage == string.Empty ? "\u200b" : character.CustomProfileMessage);

            await RespondAsync(embed: embedBuilder.Build());
        }
    }
}