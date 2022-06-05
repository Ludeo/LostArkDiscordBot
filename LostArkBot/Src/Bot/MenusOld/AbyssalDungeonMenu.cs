using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.MenusOld
{
    internal class AbyssalDungeonMenu
    {
        public static async Task AbyssalDungeon(SocketMessageComponent component, Dictionary<string, string> eventImages)
        {
            switch (component.Data.Values.First())
            {
                case "shushire":
                    SelectMenuBuilder menu = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Dungeon")
                                             .WithCustomId("shushiredungeon")
                                             .AddOption("Demon Beast Canyon", "Demon Beast Canyon", "Item Level: 340")
                                             .AddOption("Necromancer's Origin", "Necromancer's Origin", "Item Level: 340");

                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    {
                        Title = "Shushire Abyssal Dungeon",
                        Description = "Select the Shushire Abyssal Dungeon you want to do",
                        ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
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

                case "rohendel":
                    SelectMenuBuilder menu2 = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Dungeon")
                                             .WithCustomId("rohendeldungeon")
                                             .AddOption("Hall of the Twisted Warlord", "Hall of the Twisted Warlord", "Item Level: 460")
                                             .AddOption("Hildebrandt Palace", "Hildebrandt Palace", "Item Level: 460");

                    EmbedBuilder embedBuilder2 = new EmbedBuilder()
                    {
                        Title = "Rohendel Abyssal Dungeon",
                        Description = "Select the Rohendel Abyssal Dungeon you want to do",
                        ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                        Color = Color.Teal,
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

                case "yorn":
                    SelectMenuBuilder menu3 = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Dungeon")
                                             .WithCustomId("yorndungeon")
                                             .AddOption("Road of Lament", "Road of Lament", "Item Level: 840")
                                             .AddOption("Forge of Fallen Pride", "Forge of Fallen Pride", "Item Level: 840");

                    EmbedBuilder embedBuilder3 = new EmbedBuilder()
                    {
                        Title = "Yorn Abyssal Dungeon",
                        Description = "Select the Yorn Abyssal Dungeon you want to do",
                        ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                        Color = Color.Teal,
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

                case "feiton":
                    SelectMenuBuilder menu4 = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Dungeon")
                                             .WithCustomId("feitondungeon")
                                             .AddOption("Sea of Indolence", "Sea of Indolence", "Item Level: 960")
                                             .AddOption("Tranquil Karkosa", "Tranquil Karkosa", "Item Level: 960")
                                             .AddOption("Alaric's Sanctuary", "Alaric's Sanctuary", "Item Level: 960");

                    EmbedBuilder embedBuilder4 = new EmbedBuilder()
                    {
                        Title = "Feiton Abyssal Dungeon",
                        Description = "Select the Feiton Abyssal Dungeon you want to do",
                        ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                        Color = Color.Teal,
                    };

                    if (component.Message.Embeds.First().Footer is not null)
                    {
                        embedBuilder4.Footer = new EmbedFooterBuilder()
                        {
                            Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                        };
                    }

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embedBuilder4.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                    }

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder4.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu4).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });
                    break;

                case "punika":
                    SelectMenuBuilder menu5 = new SelectMenuBuilder()
                                             .WithPlaceholder("Select Dungeon")
                                             .WithCustomId("punikadungeon")
                                             .AddOption("Aira's Oculus [Normal]", "Aira's Oculus [Normal]", "Item Level: 1325")
                                             .AddOption("Oreha Preveza [Normal]", "Oreha Preveza [Normal]", "Item Level: 1340")
                                             .AddOption("Aira's Oculus [Hard]", "Aira's Oculus [Hard]", "Item Level: 1370")
                                             .AddOption("Oreha Preveza [Hard]", "Oreha Preveza [Hard]", "Item Level: 1370");

                    EmbedBuilder embedBuilder5 = new EmbedBuilder()
                    {
                        Title = "Punika Abyssal Dungeon",
                        Description = "Select the Punika Abyssal Dungeon you want to do",
                        ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                        Color = Color.Teal,
                    };

                    if (component.Message.Embeds.First().Footer is not null)
                    {
                        embedBuilder5.Footer = new EmbedFooterBuilder()
                        {
                            Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                        };
                    }

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embedBuilder5.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                    }

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder5.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu5).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });
                    break;
            }
        }
    }
}