using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class AccountModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("account", "Shows all of your registered characters")]
        public async Task Account()
        {
            ulong userId = Context.User.Id;
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            List<Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            if (characters.Count == 0)
            {
                await RespondAsync(text: "You don't have any characters registered. You can register a character with **/register**", ephemeral: true);

                return;
            }

            EmbedBuilder embed = new()
            {
                Title = "Your characters",
                Color = Color.DarkPurple,
                Description = "\u200b",
                ThumbnailUrl = Context.User.GetAvatarUrl(),
            };

            List<GuildEmote> emotes = Program.GuildEmotes;

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

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}