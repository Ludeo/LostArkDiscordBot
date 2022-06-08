using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.UserCommands
{
    public class CharactersModule : InteractionModuleBase<SocketInteractionContext<SocketUserCommand>>
    {
        [UserCommand("characters")]
        public async Task AccountUserCommand(IUser user)
        {
            ulong userId = user.Id;
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            List<Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            if (characters.Count == 0)
            {
                await RespondAsync(text: "This user doesn't have any characters registered", ephemeral: true);

                return;
            }

            EmbedBuilder embed = new()
            {
                Title = "Characters assigned to " + Context.Guild.GetUser(user.Id).DisplayName,
                Color = Color.DarkPurple,
                Description = "\u200b",
                ThumbnailUrl = user.GetAvatarUrl(),
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