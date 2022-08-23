using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace LostArkBot.Bot.Buttons;

public class ConfirmDeleteButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("confirmdeletebutton")]
    public async Task ConfirmDelete()
    {
        await this.DeferAsync();

        SocketThreadChannel threadChannel;
        IMessage lfgMessage;

        if (this.Context.Channel.GetChannelType() == ChannelType.PublicThread)
        {
            threadChannel = this.Context.Channel as SocketThreadChannel;
            ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
            lfgMessage = await channel.GetMessageAsync(threadChannel.Id);
        }
        else if (this.Context.Channel.GetChannelType() == ChannelType.DM)
        {
            await this.Context.Interaction.Message.DeleteAsync();

            return;
        }
        else
        {
            ulong messageId = (ulong)this.Context.Interaction.Message.Reference.MessageId;
            threadChannel = this.Context.Guild.GetChannel(messageId) as SocketThreadChannel;
            lfgMessage = await this.Context.Channel.GetMessageAsync(messageId);
        }

        if (threadChannel != null)
        {
            await threadChannel.DeleteAsync();
        }

        await lfgMessage.DeleteAsync();

        if (this.Context.Channel.GetChannelType() == ChannelType.PublicThread)
        {
            return;
        }

        await this.ModifyOriginalResponseAsync(
                                               msg =>
                                               {
                                                   msg.Content = "Message deleted";
                                                   msg.Components = new ComponentBuilder().Build();
                                               });
    }
}