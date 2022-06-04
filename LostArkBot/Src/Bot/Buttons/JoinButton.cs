using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    internal class JoinButton
    {
        public static async Task Join(SocketMessageComponent component)
        {
            SelectMenuBuilder joinMenu = new SelectMenuBuilder().WithCustomId("join").WithPlaceholder("Select Character");
            ulong userId = component.User.Id;

            List<Character> characterList =
                JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));

            List<Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            if (characters is null)
            {
                await component.RespondAsync(text: "Select your character from the menu", components: new ComponentBuilder().WithSelectMenu(joinMenu).Build(), ephemeral: true);
                return;
            }

            joinMenu.AddOption("Default", $"{component.Message.Id},Default", "Uses only your discord name, no additional informations");

            foreach (Character character in characters)
            {
                joinMenu.AddOption(character.CharacterName, $"{component.Message.Id},{character.CharacterName}", $"{character.ClassName}, {character.ItemLevel}");
            }

            await component.RespondAsync(text: "Select your character from the menu", components: new ComponentBuilder().WithSelectMenu(joinMenu).Build(), ephemeral: true);    
        }
    }
}