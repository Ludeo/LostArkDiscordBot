using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class DeleteButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("deletebutton")]
        public async Task Delete()
        {
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
                lfgMessage = Context.Interaction.Message;
                threadChannel = Context.Guild.GetChannel(lfgMessage.Id) as SocketThreadChannel;
            }

            if (Context.User.Id != lfgMessage.Interaction.User.Id && !Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                await RespondAsync(ephemeral: true, text: "You don't have permissions to delete this event!");
                return;
            }

            MessageComponent followupComponent = new ComponentBuilder().WithButton(Program.StaticObjects.ConfirmDeleteButton).WithButton(Program.StaticObjects.CancelButton).Build();
            await Context.Interaction.RespondAsync("Are you sure you want to delete this?", components: followupComponent, ephemeral: true);

            return;
        }
    }
}