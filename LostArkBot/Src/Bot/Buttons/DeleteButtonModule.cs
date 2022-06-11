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
            if (Context.User.Id != Context.Interaction.User.Id && !Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                await RespondAsync(ephemeral: true, text: "You don't have permissions to delete this event!");
                return;
            }

            if(Context.Channel.GetChannelType() == ChannelType.PublicThread)
            {
                SocketThreadChannel threadChannel = Context.Channel as SocketThreadChannel;
                ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
                IMessage lfgMessage = await channel.GetMessageAsync(threadChannel.Id);
                await lfgMessage.DeleteAsync();
                await threadChannel.DeleteAsync();

            } else
            {
                await Context.Interaction.Message.DeleteAsync();

                if (Context.Guild.GetChannel(Context.Interaction.Message.Id) is IThreadChannel threadChannel)
                {
                    await threadChannel.DeleteAsync();
                }
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
    }
}