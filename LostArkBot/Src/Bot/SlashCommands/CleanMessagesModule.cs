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
        [SlashCommand("clean", "Delete bot DM's (14 day limit)")]
        public async Task CleanMessages([Summary("number", "Number of messages to delete")] int numberOfMessages = 1)
        {

            if (Context.Interaction.Channel.GetChannelType() != ChannelType.DM)
            {
                if (!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
                {
                    await RespondAsync("This command can only be used in DM's", ephemeral: true);
                    return;
                }
            }

            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(numberOfMessages).FlattenAsync();
            var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

            if (filteredMessages.ToList().Count() == 0)
            {
                await RespondAsync("Nothing to delete.", ephemeral: true);
                return;
            }

            await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
            await Context.Interaction.DeferAsync();
        }
    }
}