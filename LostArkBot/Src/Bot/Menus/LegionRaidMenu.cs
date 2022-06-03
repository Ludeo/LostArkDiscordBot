using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Menus
{
    internal class LegionRaidMenu
    {
        public static async Task LegionRaid(SocketMessageComponent component, Dictionary<string, string> eventImages)
        {
            switch (component.Data.Values.First())
            {
                case "valtan":
                    SelectMenuBuilder menu = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Valtan Gate and Difficulty")
                                             .WithCustomId("valtan")
                                             .AddOption("Valtan Full [Normal]", "Valtan Full [Normal]", "Item Level: 1415")
                                             .AddOption("Valtan Gate 1 [Normal]", "Valtan Gate 1 [Normal]", "Item Level: 1415")
                                             .AddOption("Valtan Gate 2 [Normal]", "Valtan Gate 2 [Normal]", "Item Level: 1415")
                                             .AddOption("Valtan Full [Hard]", "Valtan Full [Hard]", "Item Level: 1445")
                                             .AddOption("Valtan Gate 1 [Hard]", "Valtan Gate 1 [Hard]", "Item Level: 1445")
                                             .AddOption("Valtan Gate 2 [Hard]", "Valtan Gate 2 [Hard]", "Item Level: 1415");

                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    {
                        Title = "Valtan",
                        Description = "Select the Valtan Gate and Difficulty you want to do",
                        ThumbnailUrl = StaticObjects.legionRaidIconUrl,
                        Color = Color.Teal,
                    };

                    if (component.Message.Embeds.First().Footer is not null)
                    {
                        embedBuilder.Footer = new EmbedFooterBuilder()
                        {
                            Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                        };
                    }

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embedBuilder.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                    }

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });
                    break;
            }
        }
    }
}