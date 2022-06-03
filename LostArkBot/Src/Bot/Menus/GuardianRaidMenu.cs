using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Menus
{
    internal class GuardianRaidMenu
    {
        public static async Task GuardianRaid(SocketMessageComponent component, Dictionary<string, string> eventImages)
        {
            switch (component.Data.Values.First())
            {
                case "t1guardianraid":
                    SelectMenuBuilder menu = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Guardian")
                                             .WithCustomId("t1guardianraid")
                                             .AddOption("Ur'nil", "Ur'nil", "Item Level: 302")
                                             .AddOption("Lumerus", "Lumerus", "Item Level: 340")
                                             .AddOption("Icy Legeros", "Icy Legeros", "Item Level: 380")
                                             .AddOption("Vertus", "Vertus", "Item Level: 420")
                                             .AddOption("Chromanium", "Chromanium", "Item Level: 460")
                                             .AddOption("Nacrasena", "Nacrasena", "Item Level: 500")
                                             .AddOption("Flame Fox Yoho", "Flame Fox Yoho", "Item Level: 540")
                                             .AddOption("Tytalos", "Tytalos", "Item Level: 580");

                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    {
                        Title = "Tier 1 Guardian Raid",
                        Description = "Select the Tier 1 Guardian Raid you want to do",
                        ThumbnailUrl = StaticObjects.guardianIconUrl,
                        Color = Color.Red,
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

                case "t2guardianraid":
                    SelectMenuBuilder menu2 = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Guardian")
                                             .WithCustomId("t2guardianraid")
                                             .AddOption("Dark Legoros", "Dark Legoros", "Item Level: 802")
                                             .AddOption("Helgia", "Helgia", "Item Level: 840")
                                             .AddOption("Calventus", "Calventus", "Item Level: 880")
                                             .AddOption("Achates", "Achates", "Item Level: 920")
                                             .AddOption("Frost Helgia", "Frost Helgia", "Item Level: 960")
                                             .AddOption("Lava Chromanium", "Lava Chromanium", "Item Level: 1000")
                                             .AddOption("Levanos", "Levanos", "Item Level: 1040")
                                             .AddOption("Alberhastic", "Alberhastic", "Item Level: 1080");

                    EmbedBuilder embedBuilder2 = new EmbedBuilder()
                    {
                        Title = "Tier 2 Guardian Raid",
                        Description = "Select the Tier 2 Guardian Raid you want to do",
                        ThumbnailUrl = StaticObjects.guardianIconUrl,
                        Color = Color.Red,
                    };

                    if (component.Message.Embeds.First().Footer is not null)
                    {
                        embedBuilder2.Footer = new EmbedFooterBuilder()
                        {
                            Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                        };
                    }

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embedBuilder2.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                    }

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder2.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu2).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });

                    break;

                case "t3guardianraid":
                    SelectMenuBuilder menu3 = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Guardian")
                                             .WithCustomId("t3guardianraid")
                                             .AddOption("Armored Nacrasena", "Armored Nacrasena", "Item Level: 1302")
                                             .AddOption("Igrexion", "Igrexion", "Item Level: 1340")
                                             .AddOption("Night Fox Yoho", "Night Fox Yoho", "Item Level: 1370")
                                             .AddOption("Velganos", "Velganos", "Item Level: 1385")
                                             .AddOption("Deskaluda", "Deskaluda", "Item Level: 1415");

                    EmbedBuilder embedBuilder3 = new EmbedBuilder()
                    {
                        Title = "Tier 3 Guardian Raid",
                        Description = "Select the Tier 3 Guardian Raid you want to do",
                        ThumbnailUrl = StaticObjects.guardianIconUrl,
                        Color = Color.Red,
                    };

                    if (component.Message.Embeds.First().Footer is not null)
                    {
                        embedBuilder3.Footer = new EmbedFooterBuilder()
                        {
                            Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                        };
                    }

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embedBuilder3.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                    }

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder3.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu3).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });

                    break;
            }
        }
    }
}