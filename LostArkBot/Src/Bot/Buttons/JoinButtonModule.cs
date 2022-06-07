using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class JoinButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("join")]
        public async Task Join()
        {
            SelectMenuBuilder joinMenu = new SelectMenuBuilder().WithCustomId("join").WithPlaceholder("Select Character");
            ulong userId = Context.User.Id;

            List<Character> characterList =
                JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));

            List<Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            if (characters is null)
            {
                await RespondAsync(text: "Select your character from the menu", components: new ComponentBuilder().WithSelectMenu(joinMenu).Build(), ephemeral: true);
                return;
            }

            joinMenu.AddOption("Default", $"{Context.Interaction.Message.Id},Default", "Uses only your discord name, no additional informations");

            foreach (Character character in characters)
            {
                joinMenu.AddOption(character.CharacterName, $"{Context.Interaction.Message.Id},{character.CharacterName}", $"{character.ClassName}, {character.ItemLevel}");
            }

            await RespondAsync(text: "Select your character from the menu", components: new ComponentBuilder().WithSelectMenu(joinMenu).Build(), ephemeral: true);    
        }
    }
}