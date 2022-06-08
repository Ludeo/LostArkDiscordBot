using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class HomeButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("homebutton")]
        public async Task Home()
        {
            EmbedBuilder embed = new()
            {
                Title = "Creating a LFG Event",
                Description = "Select the Event from the menu that you would like to create",
                Color = Color.Gold,
            };

            if (Context.Interaction.Message.Embeds.First().Footer is not null)
            {
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = Context.Interaction.Message.Embeds.FirstOrDefault().Footer.Value.Text,
                };
            }

            if (Context.Interaction.Message.Embeds.First().Timestamp != null)
            {
                embed.Timestamp = Context.Interaction.Message.Embeds.First().Timestamp.Value;
            }

            await Context.Interaction.UpdateAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().WithSelectMenu(Program.StaticObjects.HomeLfg).WithButton(Program.StaticObjects.DeleteButton).Build();
            });
        }
    }
}