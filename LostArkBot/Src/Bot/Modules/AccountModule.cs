using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class AccountModule
    {
        public static async Task AccountModuleAsync(SocketSlashCommand command)
        {
            ulong userId = command.User.Id;
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            List<Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            if (characters.Count == 0)
            {
                await command.RespondAsync(text: "You don't have any characters registered. You can register a character with **/register**", ephemeral: true);

                return;
            }

            EmbedBuilder embed = new()
            {
                Title = "Your characters",
                Color = Color.DarkPurple,
                Description = "\u200b",
                ThumbnailUrl = command.User.GetAvatarUrl(),
            };

            List<GuildEmote> emotes = new(await Program.Client.GetGuild(Config.Default.Server).GetEmotesAsync());

            foreach (Character character in characters)
            {
                GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

                embed.AddField(new EmbedFieldBuilder()
                {
                    Name = character.CharacterName,
                    Value = $"<:{emote.Name}:{emote.Id}> {character.ClassName}\n{character.ItemLevel}",
                    IsInline = true,
                });
            }

            await command.RespondAsync(embed: embed.Build(), ephemeral: true);
        }

        public static async Task AccountModuleAsync(SocketUserCommand command)
        {
            ulong userId = command.Data.Member.Id;
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            List<Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            if (characters.Count == 0)
            {
                await command.RespondAsync(text: "This user doesn't have any characters registered", ephemeral: true);

                return;
            }

            EmbedBuilder embed = new()
            {
                Title = "Characters assigned to " + Program.Client.GetGuild(Config.Default.Server).GetUser(command.Data.Member.Id).DisplayName,
                Color = Color.DarkPurple,
                Description = "\u200b",
                ThumbnailUrl = command.Data.Member.GetAvatarUrl(),
            };

            List<GuildEmote> emotes = new(await Program.Client.GetGuild(Config.Default.Server).GetEmotesAsync());

            foreach (Character character in characters)
            {
                GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

                embed.AddField(new EmbedFieldBuilder()
                {
                    Name = character.CharacterName,
                    Value = $"<:{emote.Name}:{emote.Id}> {character.ClassName}\n{character.ItemLevel}",
                    IsInline = true,
                });
            }

            await command.RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}