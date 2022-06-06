using Discord;
using Discord.Interactions;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class LfgModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("lfg", "Creates an LFG event")]
        public async Task Lfg(
            [Summary("custom-message", "Custom Message that will be displayed in the LFG")] string customMessage = "",
            [Summary("time", "Time of the LFG, must have format: DD/MM hh:mm")] string time = "")
        {
            ComponentBuilder component = new ComponentBuilder().WithSelectMenu(Menus.GetHomeLfg()).WithButton(StaticObjects.deleteButton);

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
                DateTimeOffset now = DateTimeOffset.Now;
                embed.Timestamp = new DateTimeOffset(now.Year, month, day, hour, minute, 0, now.Offset);
            }

            await RespondAsync(embed: embed.Build(), components: component.Build());
        }

        //[ComponentInteraction("home-lfg")]
    }
}
