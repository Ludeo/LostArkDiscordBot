using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class ConfirmDeleteButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("confirmdeletebutton")]
        public async Task ConfirmDelete()
        {
            await Context.Interaction.DeferAsync();

            SocketThreadChannel threadChannel;
            IMessage lfgMessage;

            if (Context.Channel.GetChannelType() == ChannelType.PublicThread)
            {
                threadChannel = Context.Channel as SocketThreadChannel;
                ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
                lfgMessage = await channel.GetMessageAsync(threadChannel.Id);
            }
            else if (Context.Channel.GetChannelType() == ChannelType.DM)
            {
                await Context.Interaction.Message.DeleteAsync();
                return;
            }
            else
            {
                ulong messageID = (ulong)Context.Interaction.Message.Reference.MessageId;
                threadChannel = Context.Guild.GetChannel(messageID) as SocketThreadChannel;
                lfgMessage = await Context.Channel.GetMessageAsync(messageID);
            }

            if (threadChannel != null)
            {
                await threadChannel.DeleteAsync();
            }
            await lfgMessage.DeleteAsync();

            await Context.Interaction.ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = "Message deleted";
                msg.Components = new ComponentBuilder().Build();
            });

            return;
        }
    }
}