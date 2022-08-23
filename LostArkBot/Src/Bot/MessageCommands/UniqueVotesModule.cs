using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace LostArkBot.Bot.MessageCommands;

public class UniqueVotesModule : InteractionModuleBase<SocketInteractionContext<SocketMessageCommand>>
{
    [MessageCommand("Unique Votes")]
    public async Task UniqueVotes(IMessage initMessage)
    {
        await this.DeferAsync(true);

        IMessage message = await this.Context.Channel.GetMessageAsync(initMessage.Id);
        List<string> uniqueUsers = new();

        IEnumerable<IGuildUser> guildUsersRaw = await this.Context.Guild.GetUsersAsync().FlattenAsync();
        List<IGuildUser> guildUsers = guildUsersRaw.ToList();

        foreach (IEmote emote in message.Reactions.Keys)
        {
            IEnumerable<IUser> users = await message.GetReactionUsersAsync(emote, 100).FlattenAsync();

            foreach (IUser user in users)
            {
                string nickname = guildUsers.FirstOrDefault(x => x.Id == user.Id)?.Nickname;

                if (string.IsNullOrEmpty(nickname))
                {
                    nickname = user.Username;
                }

                if (!uniqueUsers.Contains(nickname))
                {
                    uniqueUsers.Add(nickname);
                }
            }
        }

        uniqueUsers.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));

        string allUserString = uniqueUsers.Aggregate(string.Empty, (current, user) => current + user + "\n");

        allUserString = string.IsNullOrEmpty(allUserString)
            ? "No users reacted to this message"
            : $"{uniqueUsers.Count} unique users reacted to this message:\n{allUserString}";

        await this.FollowupAsync(allUserString, ephemeral: true);
    }
}