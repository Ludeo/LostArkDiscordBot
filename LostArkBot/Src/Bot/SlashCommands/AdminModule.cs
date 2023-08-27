using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using LostArkBot.Bot.Shared;

namespace LostArkBot.Bot.SlashCommands;

[Group("admin", "Commands that only admins can use")]
public class AdminModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    [SlashCommand("pick-random-member", "Randomly picks one of the guildmates")]
    public async Task PickRandomMember()
    {
        await this.FollowupAsync();

        if (!this.Context.Guild.GetUser(this.Context.User.Id).GuildPermissions.ManageMessages
         || this.Context.Guild.Id != Config.Default.Server)
        {
            IMessage message = await this.FollowupAsync("auto-delete");
            await message.DeleteAsync();
            await this.FollowupAsync("You don't have permission to execute this command", ephemeral: true);

            return;
        }

        List<IGuildUser> guildMembers = new();
        IEnumerable<IUser> users = await this.Context.Guild.GetUsersAsync().FlattenAsync();

        SocketRole role = this.Context.Guild.Roles.First(x => x.Name == "Guildmate");
        ulong roleId = role.Id;

        foreach (IUser user1 in users)
        {
            IGuildUser user = (IGuildUser)user1;

            if (user.RoleIds.FirstOrDefault(x => x == roleId) != 0)
            {
                guildMembers.Add(user);
            }
        }

        IGuildUser winner = guildMembers[Program.Random.Next(guildMembers.Count)];

        await this.FollowupAsync($"Winner out of {guildMembers.Count} guild members: {winner.Mention}\n**Congratulations!!!!!!!!!!!!!**");
        await this.Context.Channel.SendMessageAsync($"<@&{roleId}>");
    }

    [SlashCommand("set-as-merchant-channel", "Select THIS channel to post merchant items")]
    public async Task SetMerchantChannel()
    {
        await this.DeferAsync(true);

        if (this.Context.User.Id != Config.Default.Admin)
        {
            await this.FollowupAsync("You don't have permission to execute this command", ephemeral: true);

            return;
        }

        Config config = Config.Default;
        config.MerchantChannel = this.Context.Channel.Id;
        await JsonParsers.WriteConfigAsync(config);

        Program.MerchantChannel = this.Context.Channel as SocketTextChannel;
        Program.ReinitializeScheduledTasks();

        await this.FollowupAsync($"Channel {Program.MerchantChannel.Name} is now a merchant channel", ephemeral: true);
    }
}