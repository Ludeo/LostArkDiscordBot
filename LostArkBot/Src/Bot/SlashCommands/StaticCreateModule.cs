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
    public class StaticCreateModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("staticcreate", "Creates a Static Group")]
        public async Task StaticCreate(
            [Summary("name", "Name for the Static Group")] string name,
            [Summary("character-name", "Name of your character")] string characterName)
        {
            List<StaticGroup> staticGroups = JsonSerializer.Deserialize<List<StaticGroup>>(File.ReadAllText("staticgroups.json"));

            if(staticGroups.Any(x => x.Name == name))
            {
                await RespondAsync(text: "A static group with this name exists already.", ephemeral: true);
                return;
            }

            List<Character> characters = JsonSerializer.Deserialize<List<Character>>(File.ReadAllText("characters.json"));

            if(!characters.Any(x => x.CharacterName == characterName))
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
            File.WriteAllText("staticgroups.json", JsonSerializer.Serialize(staticGroups));

            await RespondAsync(text: name + " got successfully registered");
        }
    }
}