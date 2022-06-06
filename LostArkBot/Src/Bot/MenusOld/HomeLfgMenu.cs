using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.MenusOld
{
    internal class HomeLfgMenu
    {
        public static async Task HomeLfg(SocketMessageComponent component, Dictionary<string, string> eventImages, List<HomeLfgModel> modelList)
        {
            HomeLfgModel model = modelList.Find(x => x.EventId == component.Data.Values.First());

            EmbedBuilder embedBuilder = new()
            {
                Title = model.Title,
                Description = "Select the Tier of the Guardian",
                ThumbnailUrl = StaticObjects.guardianIconUrl,
                Color = Color.Red,
            };

            switch (component.Data.Values.First())
            {
                case "guardianraid":

                    EmbedBuilder embedBuilder = new()
                    {
                        Title = model.Title,
                        Description = "Select the Tier of the Guardian",
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

                    SelectMenuBuilder menu = new SelectMenuBuilder()
                                             .WithPlaceholder("Tier of Guardian")
                                             .WithCustomId("guardianraid")
                                             .AddOption("Tier 1", "t1guardianraid")
                                             .AddOption("Tier 2", "t2guardianraid")
                                             .AddOption("Tier 3", "t3guardianraid");

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });

                    break;

                case "abyssdungeon":
                    EmbedBuilder embedBuilder2 = new()
                    {
                        Title = "Abyssal Dungeon",
                        Description = "Select the Continent of the Dungeon",
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

                    SelectMenuBuilder menu2 = new SelectMenuBuilder()
                        .WithPlaceholder("Select Continent")
                        .WithCustomId("abyssdungeon")
                        .AddOption("Shushire", "shushire", "iLvl: 340")
                        .AddOption("Rohendel", "rohendel", "iLvl: 460")
                        .AddOption("Yorn", "yorn", "iLvl: 840")
                        .AddOption("Feiton", "feiton", "iLvl: 960")
                        .AddOption("Punika", "punika", "iLvl: 1325-1370");

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder2.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu2).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });

                    break;

                case "abyssraid":
                    EmbedBuilder embedBuilder3 = new()
                    {
                        Title = "Abyssal Raid",
                        Description = "Select the Abyssal Raid you want to do",
                        ThumbnailUrl = StaticObjects.abyssRaidIconUrl,
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

                    SelectMenuBuilder menu3 = new SelectMenuBuilder()
                        .WithPlaceholder("Select Abyss Raid")
                        .WithCustomId("abyssraid")
                        .AddOption("Argos", "argos", "iLvl: 1370-1400");

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder3.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu3).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });

                    break;

                case "cube":
                    EmbedBuilder embedBuilder4 = new()
                    {
                        Title = "Cube",
                        Description = "Select the Cube Tier you want to do",
                        ThumbnailUrl = StaticObjects.cubeIconUrl,
                        Color = Color.Red,
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

                    SelectMenuBuilder menu4 = new SelectMenuBuilder()
                        .WithPlaceholder("Select Tier")
                        .WithCustomId("cube")
                        .AddOption("Tier 1", "Cube", "iLvl: 302")
                        .AddOption("Tier 2", "Elite Cube", "iLvl: 802")
                        .AddOption("Tier 3", "Dimensional Cube", "iLvl: 1302");

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder4.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu4).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });

                    break;

                case "bossrush":
                    EmbedBuilder embedBuilder5 = new()
                    {
                        Title = "Boss Rush",
                        Description = "Select the Boss Rush Tier you want to do",
                        ThumbnailUrl = StaticObjects.bossRushIconUrl,
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

                    SelectMenuBuilder menu5 = new SelectMenuBuilder()
                        .WithPlaceholder("Select Tier")
                        .WithCustomId("bossrush")
                        .AddOption("Tier 2", "Hall of Silence", "iLvl: 802")
                        .AddOption("Tier 3", "Hall of the Sun", "iLvl: 1302");

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder5.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu5).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });
                    break;

                case "platinumfields":
                    EmbedBuilder embedBuilder6 = new()
                    {
                        Title = "Platinum Fields",
                        Description = "Select which Platinum Field you want to do",
                        ThumbnailUrl = StaticObjects.platinumFieldsIconUrl,
                        Color = Color.Green,
                    };

                    if (component.Message.Embeds.First().Footer is not null)
                    {
                        embedBuilder6.Footer = new EmbedFooterBuilder()
                        {
                            Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                        };
                    }

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embedBuilder6.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                    }

                    SelectMenuBuilder menu6 = new SelectMenuBuilder()
                        .WithPlaceholder("Select Platinum Field")
                        .WithCustomId("platinumfields")
                        .AddOption("Nahun's Domain", "Nahun's Domain", "Foraging/Logging/Mining")
                        .AddOption("Old Yudian Canal", "Old Yudian Canal", "Hunting/Fishing/Excavating");

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder6.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu6).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });
                    break;

                case "chaosmaps":
                    EmbedBuilder embedBuilder7 = new()
                    {
                        Title = "Chaos Maps",
                        Description = "Select which Tier of Chaos Maps you want to do",
                        ThumbnailUrl = StaticObjects.chaosMapsIconUrl,
                        Color = Color.Gold,
                    };

                    if (component.Message.Embeds.First().Footer is not null)
                    {
                        embedBuilder7.Footer = new EmbedFooterBuilder()
                        {
                            Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                        };
                    }

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embedBuilder7.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                    }

                    SelectMenuBuilder menu7 = new SelectMenuBuilder()
                        .WithPlaceholder("Select Tier")
                        .WithCustomId("chaosmaps")
                        .AddOption("Tier 1", "Tier 1", "iLvl: 302")
                        .AddOption("Tier 2", "Tier 2", "iLvl: 802")
                        .AddOption("Tier 3", "Tier 3", "iLvl: 1302");

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embedBuilder7.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu7).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });

                    break;

                case "eventguardianraid":
                    string customMessage8 = component.Message.Embeds.First().Footer == null ? null : component.Message.Embeds.First().Footer.Value.Text;

                    EmbedBuilder embed8 = new EmbedBuilder()
                    {
                        Title = "Event Guardian Raid (0/4)",
                        Description = "Waiting for members to join",
                        Author = new EmbedAuthorBuilder()
                                     .WithName($"Party Leader: {component.User.Username}")
                                     .WithIconUrl(Program.Client.GetUser(component.User.Id).GetAvatarUrl()),
                        ThumbnailUrl = StaticObjects.guardianIconUrl,
                        ImageUrl = eventImages["eventGuardianRaid"],
                        Color = Color.Red,
                    };

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embed8.AddField(new EmbedFieldBuilder()
                        {
                            Name = "Time",
                            Value = $"<t:{component.Message.Embeds.First().Timestamp.Value.ToUnixTimeSeconds()}:F>"
                        });
                    }

                    if (!string.IsNullOrEmpty(customMessage8))
                    {
                        embed8.AddField(new EmbedFieldBuilder()
                        {
                            Name = "Custom Message",
                            Value = customMessage8,
                        });
                    }

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embed8.Build();

                        x.Components = new ComponentBuilder()
                        .WithButton(StaticObjects.joinButton)
                        .WithButton(StaticObjects.leaveButton)
                        .WithButton(StaticObjects.deleteButton)
                        .WithButton(StaticObjects.startButton)
                        .Build();
                    });

                    ITextChannel textChannel8 = (ITextChannel)component.Message.Channel;
                    IThreadChannel threadChannel = await textChannel8.CreateThreadAsync(name: "Event Guardian Raid", message: component.Message, autoArchiveDuration: ThreadArchiveDuration.OneDay);

                    List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));

                    ThreadLinkedMessage threadLinkedMessage = new()
                    {
                        ThreadId = threadChannel.Id,
                        MessageId = component.Message.Id
                    };

                    threadLinkedMessageList.Add(threadLinkedMessage);
                    File.WriteAllText("ThreadMessageLink.json", JsonSerializer.Serialize(threadLinkedMessageList));

                    break;

                case "coopbattle":
                    string customMessage9 = component.Message.Embeds.First().Footer == null ? null : component.Message.Embeds.First().Footer.Value.Text;

                    EmbedBuilder embed9 = new EmbedBuilder()
                    {
                        Title = "Coop Battle (0/6)",
                        Description = "Waiting for members to join",
                        Author = new EmbedAuthorBuilder()
                                     .WithName($"Party Leader: {component.User.Username}")
                                     .WithIconUrl(Program.Client.GetUser(component.User.Id).GetAvatarUrl()),
                        ThumbnailUrl = StaticObjects.coopBattleIconUrl,
                        ImageUrl = eventImages["coopBattle"],
                        Color = Color.Red,
                    };

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embed9.AddField(new EmbedFieldBuilder()
                        {
                            Name = "Time",
                            Value = $"<t:{component.Message.Embeds.First().Timestamp.Value.ToUnixTimeSeconds()}:F>"
                        });
                    }

                    if (!string.IsNullOrEmpty(customMessage9))
                    {
                        embed9.AddField(new EmbedFieldBuilder()
                        {
                            Name = "Custom Message",
                            Value = customMessage9,
                        });
                    }

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embed9.Build();

                        x.Components = new ComponentBuilder()
                        .WithButton(StaticObjects.joinButton)
                        .WithButton(StaticObjects.leaveButton)
                        .WithButton(StaticObjects.deleteButton)
                        .WithButton(StaticObjects.startButton)
                        .Build();
                    });

                    ITextChannel textChannel9 = (ITextChannel)component.Message.Channel;
                    IThreadChannel threadChannel1 = await textChannel9.CreateThreadAsync(name: "Coop Battle", message: component.Message, autoArchiveDuration: ThreadArchiveDuration.OneDay);

                    List<ThreadLinkedMessage> threadLinkedMessageList2 = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));

                    ThreadLinkedMessage threadLinkedMessage2 = new()
                    {
                        ThreadId = threadChannel1.Id,
                        MessageId = component.Message.Id
                    };

                    threadLinkedMessageList2.Add(threadLinkedMessage2);
                    File.WriteAllText("ThreadMessageLink.json", JsonSerializer.Serialize(threadLinkedMessageList2));

                    break;

                case "legionraid":
                    EmbedBuilder embed10 = new()
                    {
                        Title = "Legion Raid",
                        Description = "Select the Legion Commander you want to do",
                        ThumbnailUrl = StaticObjects.legionRaidIconUrl,
                        Color = Color.Teal,
                    };

                    if (component.Message.Embeds.First().Footer is not null)
                    {
                        embed10.Footer = new EmbedFooterBuilder()
                        {
                            Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                        };
                    }

                    if (component.Message.Embeds.First().Timestamp != null)
                    {
                        embed10.Timestamp = component.Message.Embeds.First().Timestamp.Value;
                    }

                    SelectMenuBuilder menu8 = new SelectMenuBuilder()
                        .WithPlaceholder("Select Legion Commander")
                        .WithCustomId("legionraid")
                        .AddOption("Valtan", "valtan", "iLvl: 1415-1445");

                    await component.UpdateAsync(x =>
                    {
                        x.Embed = embed10.Build();

                        x.Components = new ComponentBuilder().WithSelectMenu(menu8).WithButton(StaticObjects.homeButton).WithButton(StaticObjects.deleteButton).Build();
                    });

                    break;
            }
        }
    }
}