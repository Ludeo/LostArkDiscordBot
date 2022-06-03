using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    internal class HomeButton
    {
        public static async Task Home(SocketMessageComponent component)
        {
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                                                    .WithPlaceholder("Select event")
                                                    .WithCustomId("home-lfg")
                                                    .AddOption("Guardian Raid", "guardianraid")
                                                    .AddOption("Abyssal Dungeon", "abyssdungeon")
                                                    .AddOption("Abyssal Raid", "abyssraid")
                                                    .AddOption("Cube Dungeon", "cube")
                                                    .AddOption("Boss Rush", "bossrush")
                                                    .AddOption("Platinum Fields", "platinum")
                                                    .AddOption("Chaos Maps", "maps");

            EmbedBuilder embed = new()
            {
                Title = "Creating a LFG Event",
                Description = "Select the Event from the menu that you would like to create",
                Color = Color.Gold,
            };

            if (component.Message.Embeds.First().Footer is not null)
            {
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = component.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                };
            }

            if (component.Message.Embeds.First().Timestamp != null)
            {
                embed.Timestamp = component.Message.Embeds.First().Timestamp.Value;
            }

            await component.UpdateAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().WithSelectMenu(menuBuilder).WithButton(StaticObjects.deleteButton).Build();
            });
        }
    }
}