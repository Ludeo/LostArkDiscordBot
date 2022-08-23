using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace LostArkBot.Bot.Buttons;

public class DeleteButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("deletebutton")]
    public async Task Delete()
    {
        await this.DeferAsync(true);

        IMessage lfgMessage;

        if (this.Context.Channel.GetChannelType() == ChannelType.PublicThread)
        {
            SocketThreadChannel threadChannel = this.Context.Channel as SocketThreadChannel;
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
            lfgMessage = this.Context.Interaction.Message;
        }

        ulong authorId = ulong.Parse(lfgMessage.Embeds.First().Author!.Value.Name.Split("\n")[1]);

        if (this.Context.User.Id != authorId
         && !this.Context.Guild.GetUser(this.Context.User.Id).GuildPermissions.ManageMessages)
        {
            await this.FollowupAsync("You don't have permissions to delete this event!", ephemeral: true);

            return;
        }

        MessageComponent followupComponent = new ComponentBuilder().WithButton(Program.StaticObjects.ConfirmDeleteButton)
                                                                   .WithButton(Program.StaticObjects.CancelButton).Build();

        await this.FollowupAsync("Are you sure you want to delete this?", components: followupComponent, ephemeral: true);
    }
}