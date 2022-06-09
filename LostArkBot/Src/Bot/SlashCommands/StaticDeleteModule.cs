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
    public class StaticDeleteModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("staticdelete", "Deletes a Static Group")]
        public async Task StaticDelete([Summary("name", "Name of the Static Group")] string name)
        {
            List<StaticGroup> staticGroups = JsonSerializer.Deserialize<List<StaticGroup>>(File.ReadAllText("staticgroups.json"));

            if(!staticGroups.Any(x => x.Name == name))
            {
                await RespondAsync(text: "There is no static group with this name", ephemeral: true);
                return;
            }

            StaticGroup staticGroup = staticGroups.Find(x => x.Name == name);

            if(Context.User.Id != staticGroup.LeaderId)
            {
                await RespondAsync(text: "You are not the leader of the group and therefore can't delete it", ephemeral: true);
                return;
            }

            staticGroups.Remove(staticGroup);
            File.WriteAllText("staticgroups.json", JsonSerializer.Serialize(staticGroups));

            await RespondAsync(text: name + " got successfully deleted", ephemeral: true);
        }
    }
}