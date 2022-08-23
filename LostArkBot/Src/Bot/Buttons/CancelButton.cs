using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace LostArkBot.Bot.Buttons;

public class CancelButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("cancelbutton")]
    public async Task Cancel()
    {
        await this.DeferAsync();

        await this.ModifyOriginalResponseAsync(
                                               msg =>
                                               {
                                                   msg.Content = "Interaction canceled";
                                                   msg.Components = new ComponentBuilder().Build();
                                               });
    }
}