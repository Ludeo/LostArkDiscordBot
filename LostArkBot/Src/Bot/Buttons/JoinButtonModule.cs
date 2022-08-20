using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class JoinButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("joinbutton")]
        public async Task Join()
        {
            await DeferAsync(ephemeral: true);

            SelectMenuBuilder joinMenu = new SelectMenuBuilder().WithCustomId("join").WithPlaceholder("Select Character");
            ulong userId = Context.User.Id;

            List<Character> characterList = await JsonParsers.GetCharactersFromJsonAsync();

            List <Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            joinMenu.AddOption("Default", "Default", "Uses only your discord name, no additional informations");

            List<GuildEmote> emotes = Program.GuildEmotes;

            foreach (Character character in characters)
            {
                GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

                joinMenu.AddOption(character.CharacterName, character.CharacterName, $"{character.ClassName}, {character.ItemLevel}", emote);
            }

            await FollowupAsync(text: "Select your character from the menu", components: new ComponentBuilder().WithSelectMenu(joinMenu).Build(), ephemeral: true);    
        }
    }
}