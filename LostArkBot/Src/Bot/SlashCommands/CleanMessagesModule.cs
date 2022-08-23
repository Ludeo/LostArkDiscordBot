using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Bot.Shared;

namespace LostArkBot.Bot.SlashCommands;

public class CleanMessagesModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    [SlashCommand("clean", "Delete messages in channel (Admin) or DMs")]
    public async Task CleanMessages([Summary("number", "Number of messages to delete")] int numberOfMessages = 1)
    {
        await this.DeferAsync(true);

        if (this.Context.Interaction.Channel.GetChannelType() != ChannelType.DM)
        {
            if (!this.Context.Guild.GetUser(this.Context.User.Id).GuildPermissions.ManageMessages)
            {
                await this.FollowupAsync("This command can only be used outside of DMs by admins", ephemeral: true);

                return;
            }
        }

        IEnumerable<IMessage> messages = await this.Context.Channel.GetMessagesAsync(numberOfMessages).FlattenAsync();
        IEnumerable<IMessage> newerMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
        List<IMessage> olderMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays > 14).ToList();

        if (newerMessages.ToList().Count + olderMessages.ToList().Count == 0)
        {
            await this.RespondAsync("Nothing found to delete.", ephemeral: true);

            return;
        }

        if (newerMessages.ToList().Count != 0)
        {
            try
            {
                if (this.Context.Channel.GetChannelType() == ChannelType.DM)
                {
                    foreach (IMessage msg in newerMessages)
                    {
                        olderMessages.Add(msg);
                    }
                }
                else
                {
                    await (this.Context.Channel as ITextChannel).DeleteMessagesAsync(newerMessages);
                }
            }
            catch (Exception e)
            {
                await LogService.Log(LogSeverity.Error, this.GetType().Name, e.ToString());
            }
        }

        if (olderMessages.ToList().Count != 0)
        {
            olderMessages.ForEach(this.TryToDeleteMessage);
        }

        await this.FollowupAsync("Messages deleted", ephemeral: true);
    }

    private async void TryToDeleteMessage(IMessage msg)
    {
        try
        {
            await msg.DeleteAsync();
        }
        catch (Exception e)
        {
            await LogService.Log(LogSeverity.Error, this.GetType().Name, e.ToString());
        }
    }
}