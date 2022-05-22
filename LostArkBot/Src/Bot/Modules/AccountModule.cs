using Discord;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            if (characters is null)
            {
                await command.RespondAsync(text: "You don't have any characters registered. You can register a character with **/register**", ephemeral: true);

                return;
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Your characters",
                Color = Color.DarkPurple,
                Description = "\n",
            };

            foreach (Character character in characters)
            {
                embed.Description += character.CharacterName + "\n";
            }

            await command.RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}