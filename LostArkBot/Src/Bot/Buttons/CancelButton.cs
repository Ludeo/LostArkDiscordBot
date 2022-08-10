using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class CancelButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("cancelbutton")]
        public async Task Cancel()
        {
            await Context.Interaction.DeferAsync();

            await Context.Interaction.ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = "Interaction canceled";
                msg.Components = new ComponentBuilder().Build();
            });

            return;
        }
    }
}