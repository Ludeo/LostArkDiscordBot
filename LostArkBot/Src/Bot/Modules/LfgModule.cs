using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;

namespace LostArkBot.Bot.Modules
{
    public class LfgModule
    {
        public static async Task LfgModuleAsync(SocketSlashCommand command)
        {
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                                            .WithPlaceholder("Select event")
                                            .WithCustomId("home-lfg")
                                            .AddOption("Guardian Raid", "guardianraid")
                                            .AddOption("Abyssal Dungeon", "abyssdungeon")
                                            .AddOption("Abyssal Raid", "abyssraid")
                                            .AddOption("Legion Raid", "legionraid")
                                            .AddOption("Cube", "cube")
                                            .AddOption("Boss Rush", "bossrush")
                                            .AddOption("Platinum Fields", "platinumfields")
                                            .AddOption("Chaos Maps", "chaosmaps")
                                            .AddOption("Event Guardian Raid", "eventguardianraid")
                                            .AddOption("Coop Battle", "coopbattle");

            ComponentBuilder component = new ComponentBuilder().WithSelectMenu(menuBuilder).WithButton(StaticObjects.deleteButton);

            string customMessage = command.Data.Options.FirstOrDefault(x => x.Name == "custom-message")
                == null ? null : command.Data.Options.First(x => x.Name == "custom-message").Value.ToString();

            string time = command.Data.Options.FirstOrDefault(x => x.Name == "time") == null ? null : command.Data.Options.First(x => x.Name == "time").Value.ToString();

            EmbedBuilder embed = new()
            {
                Title = "Creating a LFG Event",
                Description = "Select the Event from the menu that you would like to create",
                Color = Color.Gold,
            };

            if (customMessage is not null)
            {
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = customMessage,
                };
            }

            if(time is not null)
            {
                int month = int.Parse(time.Substring(0, 2));
                int day = int.Parse(time.Substring(3, 2));
                int hour = int.Parse(time.Substring(6, 2));
                int minute = int.Parse(time.Substring(9, 2));
                DateTimeOffset now = DateTimeOffset.Now;
                embed.Timestamp = new DateTimeOffset(now.Year, month, day, hour, minute, 0, now.Offset);
            }

            await command.RespondAsync(embed: embed.Build(), components: component.Build());
        }
    }
}