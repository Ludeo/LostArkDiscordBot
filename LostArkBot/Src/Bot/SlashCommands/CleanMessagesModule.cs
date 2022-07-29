using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class CleanMessagesModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("clean", "Delete bot DMs (14 day limit)")]
        public async Task CleanMessages([Summary("number", "Number of messages to delete")] int numberOfMessages = 1)
        {

            if (Context.Interaction.Channel.GetChannelType() != ChannelType.DM)
            {
                if (!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
                {
                    await RespondAsync("This command can only be used outside of DMs by admins", ephemeral: true);
                    return;
                }
            }

            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(numberOfMessages).FlattenAsync();
            IEnumerable<IMessage> newerMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
            List<IMessage> olderMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays > 14).ToList();

            if (newerMessages.ToList().Count + olderMessages.ToList().Count == 0)
            {
                await RespondAsync("Nothing found to delete.", ephemeral: true);
                return;
            }


            if (newerMessages.ToList().Count != 0)
            {
                try
                {
                    if (Context.Channel.GetChannelType() == ChannelType.DM)
                    {
                        foreach(IMessage msg in newerMessages)
                        {
                            olderMessages.Add(msg);
                        }
                    }
                    else
                    {
                        await (Context.Channel as ITextChannel).DeleteMessagesAsync(newerMessages);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            if (olderMessages.ToList().Count != 0)
            {
                olderMessages.ForEach(async msg =>
                {
                    try
                    {
                        await msg.DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            }

            await RespondAsync("Messages deleted", ephemeral: true);
        }
    }
}