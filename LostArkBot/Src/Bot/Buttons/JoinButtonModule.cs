using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;

namespace LostArkBot.Bot.Buttons;

public class JoinButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly LostArkBotContext dbcontext;

    public JoinButtonModule(LostArkBotContext context) => this.dbcontext = context;

    [ComponentInteraction("joinbutton")]
    public async Task Join()
    {
        await this.DeferAsync(true);

        SelectMenuBuilder joinMenu = new SelectMenuBuilder().WithCustomId("join").WithPlaceholder("Select Character");
        ulong userId = this.Context.User.Id;

        List<Character> characters = this.dbcontext.Characters.Where(x => x.User.DiscordUserId == userId).ToList();

        joinMenu.AddOption("Default", "Default", "Uses only your discord name, no additional information");

        List<GuildEmote> emotes = Program.GuildEmotes;

        foreach (Character character in characters)
        {
            GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

            joinMenu.AddOption(character.CharacterName, character.CharacterName, $"{character.ClassName}, {character.ItemLevel}", emote);
        }

        await this.FollowupAsync(
                                 "Select your character from the menu",
                                 components: new ComponentBuilder().WithSelectMenu(joinMenu).Build(),
                                 ephemeral: true);
    }
}