using Discord;
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
    public class StaticViewModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("staticview", "Displays all static groups or members of a specific one")]
        public async Task StaticView([Summary("group-name", "Name of the static group")] string name = "")
        {
            List<StaticGroup> staticGroups = JsonSerializer.Deserialize<List<StaticGroup>>(File.ReadAllText("staticgroups.json"));

            if (string.IsNullOrEmpty(name))
            {
                EmbedBuilder embed = new()
                {
                    Title = "List of all Static Groups",
                    Color = Color.Blue,
                };

                foreach(StaticGroup staticGroup in staticGroups)
                {
                    embed.Description += $"{staticGroup.Name} (Leader: {Context.Guild.GetUser(staticGroup.LeaderId).DisplayName})\n\n";
                }

                await RespondAsync(embed: embed.Build());
            } else
            {
                if(!staticGroups.Any(x => x.Name == name))
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

                foreach(string player in staticGroup.Players)
                {
                    embed.Description += player + "\n";
                }

                await RespondAsync(embed: embed.Build());
            }
        }
    }
}