using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;

namespace LostArkBot.Bot.UserCommands;

public class CharactersModule : InteractionModuleBase<SocketInteractionContext<SocketUserCommand>>
{
    private readonly LostArkBotContext dbcontext;

    public CharactersModule(LostArkBotContext dbcontext) => this.dbcontext = dbcontext;

    [UserCommand("characters")]
    public async Task AccountUserCommand(IUser user)
    {
        await this.DeferAsync(true);

        List<Character> characters = this.dbcontext.Characters.Where(x => x.User.DiscordUserId == user.Id).ToList();

        if (characters.Count == 0)
        {
            await this.FollowupAsync("This user doesn't have any characters registered", ephemeral: true);

            return;
        }

        EmbedBuilder embed = new()
        {
            Title = "Characters assigned to " + this.Context.Guild.GetUser(user.Id).DisplayName,
            Color = Color.DarkPurple,
            Description = "\u200b",
            ThumbnailUrl = user.GetAvatarUrl(),
        };

        List<GuildEmote> emotes = Program.GuildEmotes;

        foreach (Character character in characters)
        {
            GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

            embed.AddField(
                           new EmbedFieldBuilder
                           {
                               Name = character.CharacterName,
                               Value = $"<:{emote.Name}:{emote.Id}> {character.ClassName}\n{character.ItemLevel}",
                               IsInline = true,
                           });
        }

        await this.FollowupAsync(embed: embed.Build(), ephemeral: true);
    }
}