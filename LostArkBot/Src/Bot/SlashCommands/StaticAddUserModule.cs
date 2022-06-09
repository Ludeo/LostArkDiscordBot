using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class StaticAddUserModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("staticadduser", "Adds a user to a static group")]
        public async Task StaticAddUser(
            [Summary("character-name", "Name of the character that you want to add")] string characterName,
            [Summary("group-name", "Name of the static group")] string name)
        {
            List<StaticGroup> staticGroups = JsonSerializer.Deserialize<List<StaticGroup>>(File.ReadAllText("staticgroups.json"));

            if(!staticGroups.Any(x => x.Name == name))
            {
                await RespondAsync(text: "This static group does not exist", ephemeral: true);
                return;
            }

            StaticGroup staticGroup = staticGroups.Find(x => x.Name == name);

            if(Context.User.Id != staticGroup.LeaderId)
            {
                await RespondAsync(text: "You are not the leader of this static group and therefore can't add a user to it", ephemeral: true);
                return;
            }

            if(staticGroup.Players.Count == 8)
            {
                await RespondAsync(text: "This static group has 8 players already", ephemeral: true);
                return;
            }

            List<Character> characters = JsonSerializer.Deserialize<List<Character>>(File.ReadAllText("characters.json"));

            if(!characters.Any(x => x.CharacterName == characterName))
            {
                await RespondAsync(text: "This character does not exist", ephemeral: true);
                return;
            }

            staticGroups.Remove(staticGroup);
            staticGroup.Players.Add(characterName);
            staticGroups.Add(staticGroup);

            File.WriteAllText("staticgroups.json", JsonSerializer.Serialize(staticGroups));

            await RespondAsync(text: characterName + " got succesfully added to the static group", ephemeral: true);
        }
    }
}