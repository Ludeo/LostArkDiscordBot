using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace LostArkBot.Bot.Buttons;

public class HomeButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("homebutton")]
    public async Task Home()
    {
        await this.DeferAsync();

        EmbedBuilder embed = new()
        {
            Title = "Creating a LFG Event",
            Description = "Select the Event from the menu that you would like to create",
            Color = Color.Gold,
        };

        if (this.Context.Interaction.Message.Embeds.First().Footer is not null)
        {
            embed.Footer = new EmbedFooterBuilder
            {
                Text = this.Context.Interaction.Message.Embeds.FirstOrDefault().Footer!.Value.Text,
            };
        }

        if (this.Context.Interaction.Message.Embeds.First().Timestamp != null)
        {
            embed.Timestamp = this.Context.Interaction.Message.Embeds.First().Timestamp!.Value;
        }

        await this.ModifyOriginalResponseAsync(
                                               x =>
                                               {
                                                   x.Embed = embed.Build();

                                                   x.Components = new ComponentBuilder().WithSelectMenu(Program.StaticObjects.HomeLfg)
                                                                                        .WithButton(Program.StaticObjects.DeleteButton).Build();
                                               });
    }
}