using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Linq;
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
                int day = int.Parse(time.Substring(0, 2));
                int month = int.Parse(time.Substring(3, 2));
                int hour = int.Parse(time.Substring(6, 2));
                int minute = int.Parse(time.Substring(9, 2));
                DateTimeOffset now = DateTimeOffset.Now;
                embed.Timestamp = new DateTimeOffset(now.Year, month, day, hour, minute, 0, now.Offset);
            }

            await RespondAsync(embed: embed.Build(), components: component.Build());
        }

        [ComponentInteraction("home-lfg")]
        public async Task HomeLfgMenuHandler(string value)
        {
            EmbedBuilder embed = new();
            SelectMenuBuilder menu = new();

            switch (value)
            {
                case "guardianraid":
                    embed.Title = "Guardian Raid";
                    embed.Description = "Select the Tier of the Guardian";
                    embed.ThumbnailUrl = StaticObjects.guardianIconUrl;
                    embed.Color = Color.Red;

                    menu = Menus.GetGuardianRaidTier();

                    //embed = Embeds.GetGuardianRaid();
                    //menu = Menus.GetGuardianRaid();

                    break;
                case "abyssdungeon":
                    embed.Title = "Abyssal Dungeon";
                    embed.Description = "Select the Continent of the Dungeon";
                    embed.ThumbnailUrl = StaticObjects.abyssDungeonIconUrl;
                    embed.Color = Color.Teal;

                    menu.WithPlaceholder("Select Continent")
                        .WithCustomId("abyssdungeon")
                        .AddOption("Shushire", "shushire", "iLvl: 340")
                        .AddOption("Rohendel", "rohendel", "iLvl: 460")
                        .AddOption("Yorn", "yorn", "iLvl: 840")
                        .AddOption("Feiton", "feiton", "iLvl: 960")
                        .AddOption("Punika", "punika", "iLvl: 1325-1370");

                    break;
                case "abyssraid":
                    break;
                case "legionraid":
                    break;
                case "cube":
                    break;
                case "bossrush":
                    break;
                case "platinumfield":
                    break;
                case "chaosmaps":
                    break;
                case "eventguardian":
                    break;
                case "coopbattle":
                    break;
            }

            SocketMessageComponent messageComponent = Context.Interaction as SocketMessageComponent;
            SocketUserMessage message = messageComponent.Message;

            if (message.Embeds.First().Footer is not null)
            {
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = message.Embeds.FirstOrDefault().Footer.Value.Text,
                };
            }

            if (message.Embeds.First().Timestamp != null)
            {
                embed.Timestamp = message.Embeds.First().Timestamp.Value;
            }
            await message.ModifyAsync(x => x.Embed = embed.Build());

            try
            {
                await RespondAsync();
            } catch(HttpException)
            {

            }
        }
    }
}
