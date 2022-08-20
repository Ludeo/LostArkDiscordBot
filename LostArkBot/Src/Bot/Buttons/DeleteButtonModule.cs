using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class DeleteButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("deletebutton")]
        public async Task Delete()
        {
            await DeferAsync(ephemeral: true);

            IMessage lfgMessage;

            if (Context.Channel.GetChannelType() == ChannelType.PublicThread)
            {
                SocketThreadChannel threadChannel = Context.Channel as SocketThreadChannel;
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
            }

            ulong authorId = ulong.Parse(lfgMessage.Embeds.First().Author.Value.Name.Split("\n")[1]);

            if (Context.User.Id != authorId && !Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                await FollowupAsync(text: "You don't have permissions to delete this event!", ephemeral: true);
                return;
            }

            MessageComponent followupComponent = new ComponentBuilder().WithButton(Program.StaticObjects.ConfirmDeleteButton).WithButton(Program.StaticObjects.CancelButton).Build();
            await FollowupAsync("Are you sure you want to delete this?", components: followupComponent, ephemeral: true);

            return;
        }
    }
}