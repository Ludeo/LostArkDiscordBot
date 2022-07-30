using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.MessageCommands
{
    public class UniqueVotesModule : InteractionModuleBase<SocketInteractionContext<SocketMessageCommand>>
    {
        [MessageCommand("Unique Votes")]
        public async Task UniqueVotes(IMessage initMessage)
        {
            await DeferAsync(ephemeral: true);

            IMessage message = await Context.Channel.GetMessageAsync(initMessage.Id);
            List<string> uniqueUsers = new();

            IEnumerable<IGuildUser> guildUsersRaw = await Context.Guild.GetUsersAsync().FlattenAsync();
            List<IGuildUser> guildUsers = guildUsersRaw.ToList();

            foreach (IEmote emote in message.Reactions.Keys)
            {
                IEnumerable<IUser> users = await message.GetReactionUsersAsync(emote, 100).FlattenAsync();

                foreach (IUser user in users)
                {
                    string nickname = guildUsers.FirstOrDefault(x => x.Id == user.Id)?.Nickname;

                    if(string.IsNullOrEmpty(nickname))
                    {
                        nickname = user.Username;
                    }

                    if (!uniqueUsers.Contains(nickname))
                    {
                        uniqueUsers.Add(nickname);
                    }
                }
            }

            uniqueUsers.Sort((a, b) => a.CompareTo(b));

            string allUserString = string.Empty;

            foreach(string user in uniqueUsers)
            {
                allUserString += user + "\n";
            }

            if(string.IsNullOrEmpty(allUserString))
            {
                allUserString = "No users reacted to this message";
            } else
            {
                allUserString = $"{uniqueUsers.Count} unique users reacted to this message:\n{allUserString}";
            }

            await FollowupAsync(text: allUserString);
        }
    }
}