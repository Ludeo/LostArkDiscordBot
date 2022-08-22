using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [Group("static", "Manage or view static groups")]
    public class StaticModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly LostArkBotContext dbcontext;

        public StaticModule(LostArkBotContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [SlashCommand("create", "Creates a Static Group")]
        public async Task Create(
            [Summary("name", "Name for the Static Group")] string name,
            [Summary("character-name", "Name of your character")] string characterName)
        {
            await DeferAsync();

            if (dbcontext.StaticGroups.Where(x => x.Name == name).FirstOrDefault() is not null)
            {
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: "A static group with this name exists already.", ephemeral: true);
                return;
            }

            Character character = dbcontext.Characters.Where(x => x.CharacterName == characterName).Include(x => x.User).FirstOrDefault();

            if (character is null)
            {
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: "This character does not exist. Register it with **/register** before you create a static group", ephemeral: true);
                return;
            }

            StaticGroup staticGroup = new()
            {
                Name = name,
                LeaderId = character.User.Id,
            };

            staticGroup.Characters.Add(character);
            dbcontext.StaticGroups.Add(staticGroup);
            await dbcontext.SaveChangesAsync();

            await FollowupAsync(text: name + " got successfully registered");
        }

        [SlashCommand("delete", "Deletes a Static Group")]
        public async Task Delete([Summary("name", "Name of the Static Group")] string name)
        {
            await DeferAsync(ephemeral: true);

            StaticGroup staticGroup = dbcontext.StaticGroups.Where(x => x.Name == name).Include(x => x.Leader).FirstOrDefault();

            if (staticGroup is null)
            {
                await FollowupAsync(text: "There is no static group with this name", ephemeral: true);
                return;
            }

            if (Context.User.Id != staticGroup.Leader.DiscordUserId)
            {
                await FollowupAsync(text: "You are not the leader of the group and therefore can't delete it", ephemeral: true);
                return;
            }

            dbcontext.StaticGroups.Remove(staticGroup);
            await dbcontext.SaveChangesAsync();

            await FollowupAsync(text: name + " got successfully deleted", ephemeral: true);
        }

        [SlashCommand("adduser", "Adds a user to a static group")]
        public async Task AddUser(
            [Summary("character-name", "Name of the character that you want to add")] string characterName,
            [Summary("group-name", "Name of the static group")] string name)
        {
            await DeferAsync(ephemeral: true);

            StaticGroup staticGroup = dbcontext.StaticGroups.Where(x => x.Name == name).Include(x => x.Characters).Include(x => x.Leader).FirstOrDefault();

            if (staticGroup is null)
            {
                await FollowupAsync(text: "This static group does not exist", ephemeral: true);
                return;
            }

            if (Context.User.Id != staticGroup.Leader.DiscordUserId)
            {
                await FollowupAsync(text: "You are not the leader of this static group and therefore can't add a user to it", ephemeral: true);
                return;
            }

            if (staticGroup.Characters.Count == 8)
            {
                await FollowupAsync(text: "This static group has 8 players already", ephemeral: true);
                return;
            }

            Character character = dbcontext.Characters.Where(x => x.CharacterName == characterName).FirstOrDefault();

            if (character is null)
            {
                await FollowupAsync(text: "This character does not exist", ephemeral: true);
                return;
            }

            staticGroup.Characters.Add(character);
            dbcontext.StaticGroups.Update(staticGroup);
            await dbcontext.SaveChangesAsync();

            await FollowupAsync(text: characterName + " got succesfully added to the static group", ephemeral: true);
        }

        [SlashCommand("removeuser", "Removes a user from a static group")]
        public async Task RemoveUser(
            [Summary("character-name", "Name of the character that you want to remove")] string characterName,
            [Summary("group-name", "Name of the static group")] string name)
        {
            await DeferAsync(ephemeral: true);

            StaticGroup staticGroup = dbcontext.StaticGroups.Where(x => x.Name == name).Include(x => x.Leader).FirstOrDefault();

            if (staticGroup is null)
            {
                await FollowupAsync(text: "This static group does not exist", ephemeral: true);
                return;
            }

            if (Context.User.Id != staticGroup.Leader.DiscordUserId)
            {
                await FollowupAsync(text: "You are not the leader of this static group and therefore can't remove a user from it", ephemeral: true);
                return;
            }

            Character character = dbcontext.Characters.Where(x => x.CharacterName == characterName).FirstOrDefault();

            if (character is null)
            {
                await FollowupAsync(text: "This character does not exist", ephemeral: true);
                return;
            }

            staticGroup.Characters.Remove(character);
            dbcontext.StaticGroups.Update(staticGroup);
            await dbcontext.SaveChangesAsync();

            await FollowupAsync(text: characterName + " got succesfully removed from the static group", ephemeral: true);
        }

        [SlashCommand("list", "Displays all static groups")]
        public async Task List()
        {
            await DeferAsync();

            List<StaticGroup> staticGroups = dbcontext.StaticGroups.Include(x => x.Leader).ToList();

            EmbedBuilder embed = new()
            {
                Title = "List of all Static Groups",
                Color = Color.Blue,
            };

            foreach (StaticGroup staticGroup in staticGroups)
            {
                embed.Description += $"{staticGroup.Name} (Leader: {Context.Guild.GetUser(staticGroup.Leader.DiscordUserId).DisplayName})\n\n";
            }

            await FollowupAsync(embed: embed.Build());
        }

        [SlashCommand("view", "View members of a specific static group")]
        public async Task View([Summary("group-name", "Name of the static group")] string name)
        {
            await DeferAsync();

            StaticGroup staticGroup = dbcontext.StaticGroups.Where(x => x.Name == name).Include(x => x.Characters).FirstOrDefault();

            if (staticGroup is null)
            {
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: "There is no static group with this name", ephemeral: true);
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

            await FollowupAsync(embed: embed.Build());
        }
    }
}