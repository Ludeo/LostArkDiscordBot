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
        [ComponentInteraction("joinbutton")]
        public async Task Join()
        {
            SelectMenuBuilder joinMenu = new SelectMenuBuilder().WithCustomId("join").WithPlaceholder("Select Character");
            ulong userId = Context.User.Id;

            List<Character> characterList =
                JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));

            List<Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            // I think this does nothing, it is never null
            if (characters is null)
            {
                await RespondAsync(text: "Select your character from the menu", components: new ComponentBuilder().WithSelectMenu(joinMenu).Build(), ephemeral: true);
                return;
            }

            joinMenu.AddOption("Default", "Default", "Uses only your discord name, no additional informations");

            List<GuildEmote> emotes = Program.GuildEmotes;

            foreach (Character character in characters)
            {
                GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

                joinMenu.AddOption(character.CharacterName, character.CharacterName, $"{character.ClassName}, {character.ItemLevel}", emote);
            }

            await RespondAsync(text: "Select your character from the menu", components: new ComponentBuilder().WithSelectMenu(joinMenu).Build(), ephemeral: true);    
        }
    }
}