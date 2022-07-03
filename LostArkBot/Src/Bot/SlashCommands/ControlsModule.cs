using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class ControlsModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("controls", "Posts the buttons of the lfg")]
        public async Task Controls()
        {
            if(Context.Channel.GetChannelType() != ChannelType.PublicThread)
            {
                await RespondAsync(text: "This command can only be executed in the thread channel of the lfg", ephemeral: true);
                return;
            }

            ComponentBuilder components = new ComponentBuilder().WithButton(Program.StaticObjects.JoinButton)
                                                                .WithButton(Program.StaticObjects.LeaveButton)
                                                                .WithButton(Program.StaticObjects.KickButton)
                                                                .WithButton(Program.StaticObjects.DeleteButton)
                                                                .WithButton(Program.StaticObjects.StartButton);

            await RespondAsync(components: components.Build());
        }
    }
}