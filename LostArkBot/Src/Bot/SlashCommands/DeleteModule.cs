using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class DeleteModule
    {
        public class AccountModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
        {
            [SlashCommand("delete", "Deletes the given character from your character list")]
            public async Task Delete([Summary("character-name", "Name of the character you want to delete")] string characterName)
            {
                ulong userId = Context.User.Id;
                List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
                Character character = characterList.Find(x => x.DiscordUserId == userId && x.CharacterName == characterName);

                if (character is null)
                {
                    await RespondAsync(text: $"{characterName} is not registered or it doesn't belong to you", ephemeral: true);

                    return;
                }

                characterList.Remove(character);

                await File.WriteAllTextAsync("characters.json", JsonSerializer.Serialize(characterList));

                await RespondAsync(text: $"{characterName} has been successfully deleted", ephemeral: true);
            }
        }
    }
}
