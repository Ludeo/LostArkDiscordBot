using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using Microsoft.EntityFrameworkCore;

namespace LostArkBot.Bot.SlashCommands;

[DontAutoRegister]
[Group("static", "Manage or view static groups")]
public class StaticModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    private readonly LostArkBotContext dbcontext;

    public StaticModule(LostArkBotContext dbcontext) => this.dbcontext = dbcontext;

    [SlashCommand("adduser", "Adds a user to a static group")]
    public async Task AddUser(
        [Summary("character-name", "Name of the character that you want to add")] string characterName,
        [Summary("group-name", "Name of the static group")]
        string name)
    {
        await this.DeferAsync(true);

        StaticGroup staticGroup = this.dbcontext.StaticGroups.Where(x => x.Name == name).Include(x => x.Characters).Include(x => x.Leader)
                                      .FirstOrDefault();

        if (staticGroup is null)
        {
            await this.FollowupAsync("This static group does not exist", ephemeral: true);

            return;
        }

        if (this.Context.User.Id != staticGroup.Leader.DiscordUserId)
        {
            await this.FollowupAsync("You are not the leader of this static group and therefore can't add a user to it", ephemeral: true);

            return;
        }

        if (staticGroup.Characters.Count == 8)
        {
            await this.FollowupAsync("This static group has 8 players already", ephemeral: true);

            return;
        }

        Character character = this.dbcontext.Characters.FirstOrDefault(x => x.CharacterName == characterName);

        if (character is null)
        {
            await this.FollowupAsync("This character does not exist", ephemeral: true);

            return;
        }

        staticGroup.Characters.Add(character);
        this.dbcontext.StaticGroups.Update(staticGroup);
        await this.dbcontext.SaveChangesAsync();

        await this.FollowupAsync(characterName + " got successfully added to the static group", ephemeral: true);
    }

    [SlashCommand("create", "Creates a Static Group")]
    public async Task Create(
        [Summary("name", "Name for the Static Group")] string name,
        [Summary("character-name", "Name of your character")]
        string characterName)
    {
        await this.DeferAsync();

        if (this.dbcontext.StaticGroups.FirstOrDefault(x => x.Name == name) is not null)
        {
            IMessage message = await this.FollowupAsync("auto-delete");
            await message.DeleteAsync();
            await this.FollowupAsync("A static group with this name exists already.", ephemeral: true);

            return;
        }

        Character character = this.dbcontext.Characters.Where(x => x.CharacterName == characterName).Include(x => x.User).FirstOrDefault();

        if (character is null)
        {
            IMessage message = await this.FollowupAsync("auto-delete");
            await message.DeleteAsync();

            await this.FollowupAsync(
                                     "This character does not exist. Register it with **/register** before you create a static group",
                                     ephemeral: true);

            return;
        }

        StaticGroup staticGroup = new()
        {
            Name = name,
            LeaderId = character.User.Id,
        };

        staticGroup.Characters.Add(character);
        this.dbcontext.StaticGroups.Add(staticGroup);
        await this.dbcontext.SaveChangesAsync();

        await this.FollowupAsync(name + " got successfully registered");
    }

    [SlashCommand("delete", "Deletes a Static Group")]
    public async Task Delete([Summary("name", "Name of the Static Group")] string name)
    {
        await this.DeferAsync(true);

        StaticGroup staticGroup = this.dbcontext.StaticGroups.Where(x => x.Name == name).Include(x => x.Leader).Include(x => x.Characters)
                                      .FirstOrDefault();

        if (staticGroup is null)
        {
            await this.FollowupAsync("There is no static group with this name", ephemeral: true);

            return;
        }

        if (this.Context.User.Id != staticGroup.Leader.DiscordUserId)
        {
            await this.FollowupAsync("You are not the leader of the group and therefore can't delete it", ephemeral: true);

            return;
        }

        foreach (Character character in staticGroup.Characters)
        {
            staticGroup.Characters.Remove(character);
        }

        this.dbcontext.StaticGroups.Remove(staticGroup);
        await this.dbcontext.SaveChangesAsync();

        await this.FollowupAsync(name + " got successfully deleted", ephemeral: true);
    }

    [SlashCommand("list", "Displays all static groups")]
    public async Task List()
    {
        await this.DeferAsync();

        List<StaticGroup> staticGroups = this.dbcontext.StaticGroups.Include(x => x.Leader).ToList();

        EmbedBuilder embed = new()
        {
            Title = "List of all Static Groups",
            Color = Color.Blue,
        };

        foreach (StaticGroup staticGroup in staticGroups)
        {
            embed.Description += $"{staticGroup.Name} (Leader: {this.Context.Guild.GetUser(staticGroup.Leader.DiscordUserId).DisplayName})\n\n";
        }

        await this.FollowupAsync(embed: embed.Build());
    }

    [SlashCommand("removeuser", "Removes a user from a static group")]
    public async Task RemoveUser(
        [Summary("character-name", "Name of the character that you want to remove")] string characterName,
        [Summary("group-name", "Name of the static group")]
        string name)
    {
        await this.DeferAsync(true);

        StaticGroup staticGroup = this.dbcontext.StaticGroups.Where(x => x.Name == name).Include(x => x.Leader).FirstOrDefault();

        if (staticGroup is null)
        {
            await this.FollowupAsync("This static group does not exist", ephemeral: true);

            return;
        }

        if (this.Context.User.Id != staticGroup.Leader.DiscordUserId)
        {
            await this.FollowupAsync("You are not the leader of this static group and therefore can't remove a user from it", ephemeral: true);

            return;
        }

        Character character = this.dbcontext.Characters.FirstOrDefault(x => x.CharacterName == characterName);

        if (character is null)
        {
            await this.FollowupAsync("This character does not exist", ephemeral: true);

            return;
        }

        staticGroup.Characters.Remove(character);
        this.dbcontext.StaticGroups.Update(staticGroup);
        await this.dbcontext.SaveChangesAsync();

        await this.FollowupAsync(characterName + " got successfully removed from the static group", ephemeral: true);
    }

    [SlashCommand("view", "View members of a specific static group")]
    public async Task View([Summary("group-name", "Name of the static group")] string name)
    {
        await this.DeferAsync();

        StaticGroup staticGroup = this.dbcontext.StaticGroups.Where(x => x.Name == name).Include(x => x.Characters).FirstOrDefault();

        if (staticGroup is null)
        {
            IMessage message = await this.FollowupAsync("auto-delete");
            await message.DeleteAsync();
            await this.FollowupAsync("There is no static group with this name", ephemeral: true);

            return;
        }

        EmbedBuilder embed = new()
        {
            Title = "Members of " + name,
            Color = Color.Blue,
        };

        foreach (Character character in staticGroup.Characters)
        {
            embed.Description += character.CharacterName + "\n";
        }

        await this.FollowupAsync(embed: embed.Build());
    }
}