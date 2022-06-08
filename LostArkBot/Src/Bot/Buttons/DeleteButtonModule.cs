using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class DeleteButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("deletebutton")]
        public async Task Delete()
        {
            if (Context.User.Id == Context.Interaction.User.Id || Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                await Context.Interaction.Message.DeleteAsync();

                IThreadChannel threadChannel = Context.Guild.GetChannel(Context.Interaction.Message.Id) as IThreadChannel;

                if (threadChannel != null)
                {
                    await threadChannel.DeleteAsync();
                }

                try
                {
                    await RespondAsync();
                } catch(HttpException exception)
                {
                    await Program.Log(new LogMessage(LogSeverity.Error, "DeleteButton.cs", exception.Message));
                }
                
                return;
            }

            await RespondAsync(ephemeral: true, text: "You don't have permissions to delete this event!");
        }
    }
}