using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class LfgModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("lfg", "Creates an LFG event")]
        public async Task Lfg(
            [Summary("custom-message", "Custom Message that will be displayed in the LFG")] string customMessage = "",
            [Summary("time", "Time of the LFG, must be server time and must have format: DD/MM hh:mm")] string time = "",
            [Summary("static-group", "Name of the static group")] string staticGroup = "")
        {
            if(!string.IsNullOrEmpty(staticGroup))
            {
                List<StaticGroup> staticGroups = JsonSerializer.Deserialize<List<StaticGroup>>(File.ReadAllText("staticgroups.json"));

                if(!staticGroups.Any(x => x.Name == staticGroup))
                {
                    await RespondAsync(text: "The given static group doesn't exist", ephemeral: true);
                    return;
                }
            }

            ComponentBuilder component = new ComponentBuilder().WithSelectMenu(Program.StaticObjects.HomeLfg).WithButton(Program.StaticObjects.DeleteButton);

            EmbedBuilder embed = new()
            {
                Title = "Creating a LFG Event",
                Description = "Select the Event from the menu that you would like to create",
                Color = Color.Gold,
            };

            if (!string.IsNullOrEmpty(customMessage))
            {
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = customMessage,
                };
            }

            if (!string.IsNullOrEmpty(time))
            {
                int day = int.Parse(time[..2]);
                int month = int.Parse(time.Substring(3, 2));
                int hour = int.Parse(time.Substring(6, 2));
                int minute = int.Parse(time.Substring(9, 2));
                int year = DateTimeOffset.Now.Year;

                if (month < DateTimeOffset.Now.Month)
                {
                    year += 1;
                }

                embed.Timestamp = new DateTimeOffset(year, month, day, hour, minute, 0, new TimeSpan(1, 0, 0));
            }

            await RespondAsync(text: staticGroup, embed: embed.Build(), components: component.Build());
        }
    }
}