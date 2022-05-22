using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class DeleteModule
    {
        public static async Task DeleteModuleAsync(SocketSlashCommand command)
        {
            ulong userId = command.User.Id;
            string characterName = command.Data.Options.First(x => x.Name == "character-name").Value.ToString();
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            Character character = characterList.Find(x => x.DiscordUserId == userId && x.CharacterName == characterName);

            if (character is null)
            {
                await command.RespondAsync(text: $"{characterName} is not registered or it doesn't belong to you", ephemeral: true);

                return;
            }

            characterList.Remove(character);

            await File.WriteAllTextAsync("characters.json", JsonSerializer.Serialize(characterList));

            await command.RespondAsync(text: $"{characterName} has been successfully deleted", ephemeral: true);
        }
    }
}