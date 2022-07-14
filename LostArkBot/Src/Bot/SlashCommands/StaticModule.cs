using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [Group("static", "Manage or view static groups")]
    public class StaticModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("create", "Creates a Static Group")]
        public async Task Create(
            [Summary("name", "Name for the Static Group")] string name,
            [Summary("character-name", "Name of your character")] string characterName)
        {
            List<StaticGroup> staticGroups = await JsonParsers.GetStaticGroupsFromJsonAsync();

            if (staticGroups.Any(x => x.Name == name))
            {
                await RespondAsync(text: "A static group with this name exists already.", ephemeral: true);
                return;
            }

            List<Character> characters = await JsonParsers.GetCharactersFromJsonAsync();

            if (!characters.Any(x => x.CharacterName == characterName))
            {
                await RespondAsync(text: "This character does not exist. Register it with **/register** before you create a static group", ephemeral: true);
                return;
            }

            StaticGroup staticGroup = new()
            {
                Name = name,
                LeaderId = Context.User.Id,
                Players = new()
                {
                    characterName,
                },
            };

            staticGroups.Add(staticGroup);
            await JsonParsers.WriteStaticGroupsAsync(staticGroups);

            await RespondAsync(text: name + " got successfully registered");
        }

        [SlashCommand("delete", "Deletes a Static Group")]
        public async Task Delete([Summary("name", "Name of the Static Group")] string name)
        {
            List<StaticGroup> staticGroups = await JsonParsers.GetStaticGroupsFromJsonAsync();

            if (!staticGroups.Any(x => x.Name == name))
            {
                await RespondAsync(text: "There is no static group with this name", ephemeral: true);
                return;
            }

            StaticGroup staticGroup = staticGroups.Find(x => x.Name == name);

            if (Context.User.Id != staticGroup.LeaderId)
            {
                await RespondAsync(text: "You are not the leader of the group and therefore can't delete it", ephemeral: true);
                return;
            }

            staticGroups.Remove(staticGroup);

            await JsonParsers.WriteStaticGroupsAsync(staticGroups);

            await RespondAsync(text: name + " got successfully deleted", ephemeral: true);
        }

        [SlashCommand("adduser", "Adds a user to a static group")]
        public async Task AddUser(
            [Summary("character-name", "Name of the character that you want to add")] string characterName,
            [Summary("group-name", "Name of the static group")] string name)
        {
            List<StaticGroup> staticGroups = await JsonParsers.GetStaticGroupsFromJsonAsync();

            if (!staticGroups.Any(x => x.Name == name))
            {
                await RespondAsync(text: "This static group does not exist", ephemeral: true);
                return;
            }

            StaticGroup staticGroup = staticGroups.Find(x => x.Name == name);

            if (Context.User.Id != staticGroup.LeaderId)
            {
                await RespondAsync(text: "You are not the leader of this static group and therefore can't add a user to it", ephemeral: true);
                return;
            }

            if (staticGroup.Players.Count == 8)
            {
                await RespondAsync(text: "This static group has 8 players already", ephemeral: true);
                return;
            }

            List<Character> characters = await JsonParsers.GetCharactersFromJsonAsync();

            if (!characters.Any(x => x.CharacterName == characterName))
            {
                await RespondAsync(text: "This character does not exist", ephemeral: true);
                return;
            }

            staticGroups.Remove(staticGroup);
            staticGroup.Players.Add(characterName);
            staticGroups.Add(staticGroup);

            await JsonParsers.WriteStaticGroupsAsync(staticGroups);

            await RespondAsync(text: characterName + " got succesfully added to the static group", ephemeral: true);
        }

        [SlashCommand("removeuser", "Removes a user from a static group")]
        public async Task RemoveUser(
            [Summary("character-name", "Name of the character that you want to remove")] string characterName,
            [Summary("group-name", "Name of the static group")] string name)
        {
            List<StaticGroup> staticGroups = await JsonParsers.GetStaticGroupsFromJsonAsync();

            if (!staticGroups.Any(x => x.Name == name))
            {
                await RespondAsync(text: "This static group does not exist", ephemeral: true);
                return;
            }

            StaticGroup staticGroup = staticGroups.Find(x => x.Name == name);

            if (Context.User.Id != staticGroup.LeaderId)
            {
                await RespondAsync(text: "You are not the leader of this static group and therefore can't remove a user from it", ephemeral: true);
                return;
            }

            List<Character> characters = await JsonParsers.GetCharactersFromJsonAsync();

            if (!characters.Any(x => x.CharacterName == characterName))
            {
                await RespondAsync(text: "This character does not exist", ephemeral: true);
                return;
            }

            staticGroups.Remove(staticGroup);
            staticGroup.Players.Remove(characterName);
            staticGroups.Add(staticGroup);

            await JsonParsers.WriteStaticGroupsAsync(staticGroups);

            await RespondAsync(text: characterName + " got succesfully removed from the static group", ephemeral: true);
        }

        [SlashCommand("list", "Displays all static groups")]
        public async Task List()
        {
            List<StaticGroup> staticGroups = await JsonParsers.GetStaticGroupsFromJsonAsync();

            EmbedBuilder embed = new()
            {
                Title = "List of all Static Groups",
                Color = Color.Blue,
            };

            foreach (StaticGroup staticGroup in staticGroups)
            {
                embed.Description += $"{staticGroup.Name} (Leader: {Context.Guild.GetUser(staticGroup.LeaderId).DisplayName})\n\n";
            }

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("view", "View members of a specific static group")]
        public async Task View([Summary("group-name", "Name of the static group")] string name)
        {
            List<StaticGroup> staticGroups = await JsonParsers.GetStaticGroupsFromJsonAsync();

            if (!staticGroups.Any(x => x.Name == name))
            {
                await RespondAsync(text: "There is no static group with this name", ephemeral: true);
                return;
            }

            StaticGroup staticGroup = staticGroups.Find(x => x.Name == name);

            EmbedBuilder embed = new()
            {
                Title = "Members of " + name,
                Color = Color.Blue,
            };

            foreach (string player in staticGroup.Players)
            {
                embed.Description += player + "\n";
            }

            await RespondAsync(embed: embed.Build());
        }
    }
}