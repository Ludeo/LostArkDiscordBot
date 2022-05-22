using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Menus
{
    internal class AbyssalRaidMenu
    {
        public static async Task AbyssalRaid(SocketMessageComponent component, Dictionary<string, string> eventImages)
        {
            switch (component.Data.Values.First())
            {
                case "argos":
                    SelectMenuBuilder menu = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Argos Phase")
                                             .WithCustomId("argos")
                                             .AddOption("Argos Phase 1", "Argos Phase 1", "Item Level: 1370")
                                             .AddOption("Argos Phase 2", "Argos Phase 2", "Item Level: 1385")
                                             .AddOption("Argos Phase 3", "Argos Phase 3", "Item Level: 1400");

                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    {
                        Title = "Argos",
                        Description = "Select the Argos Phase you want to do",
                        ThumbnailUrl = StaticObjects.abyssRaidIconUrl,
                        Color = Color.Teal,
                    };

                    if (component.Message.Embeds.First().Footer is not null)
                    {
                        embedBuilder.Footer = new EmbedFooterBuilder()
                        {
                            Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                        };
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