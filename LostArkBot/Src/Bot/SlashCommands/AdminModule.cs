using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using LostArkBot.Bot.Models;
using LostArkBot.Bot.Shared;
using LostArkBot.databasemodels;

namespace LostArkBot.Bot.SlashCommands;

[Group("admin", "Commands that only admins can use")]
public class AdminModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    private readonly LostArkBotContext dbcontext;

    public AdminModule(LostArkBotContext dbcontext) => this.dbcontext = dbcontext;

    [SlashCommand("edit-challenge-abyss", "Edits the list of current challenge abyss dungeons")]
    public async Task EditChallengeAbyss(
        [Summary("first", "Name of the first abyss dungeon")] string firstAbyssName,
        [Summary("second", "Name of the second abyss dungeon")]
        string secondAbyssName)
    {
        await this.DeferAsync(true);

        if (!this.Context.Guild.GetUser(this.Context.User.Id).GuildPermissions.ManageMessages
         || this.Context.Guild.Id != Config.Default.Server)
        {
            await this.FollowupAsync("You don't have permission to execute this command", ephemeral: true);

            return;
        }

        this.dbcontext.ChallengeAbysses.RemoveRange(this.dbcontext.ChallengeAbysses);

        this.dbcontext.ChallengeAbysses.Add(
                                            new ChallengeAbyss
                                            {
                                                Name = firstAbyssName,
                                            });

        this.dbcontext.ChallengeAbysses.Add(
                                            new ChallengeAbyss
                                            {
                                                Name = secondAbyssName,
                                            });

        await this.dbcontext.SaveChangesAsync();

        List<LfgModel> lfgModels = Program.StaticObjects.LfgModels;
        LfgModel oldModel = lfgModels.Find(x => x.MenuId.Contains("home-lfg") && x.MenuItemId == "challengeabyss");

        List<ChallengeAbyss> challengeAbysses = this.dbcontext.ChallengeAbysses.ToList();

        oldModel.MenuBuilderOptions = new List<MenuBuilderOption>
        {
            new(challengeAbysses[0].Name, challengeAbysses[0].Name),
            new(challengeAbysses[1].Name, challengeAbysses[1].Name),
            new("Both Abysses", "Both Abysses"),
        };

        lfgModels.Remove(oldModel);
        lfgModels.Add(oldModel);
        Program.StaticObjects.LfgModels = lfgModels;

        await this.FollowupAsync("Challenge Abyss updated!", ephemeral: true);
    }

    [SlashCommand("edit-challenge-guardian", "Edits the list of current challenge guardians")]
    public async Task EditChallengeGuardian([Summary("name", "Name of the guardian")] string guardianName)
    {
        await this.DeferAsync(true);

        if (!this.Context.Guild.GetUser(this.Context.User.Id).GuildPermissions.ManageMessages
         || this.Context.Guild.Id != Config.Default.Server)
        {
            await this.FollowupAsync("You don't have permission to execute this command", ephemeral: true);

            return;
        }

        foreach (ChallengeGuardian guardian in this.dbcontext.ChallengeGuardians.ToList())
        {
            if (guardian.WeekNumber == 3)
            {
                this.dbcontext.ChallengeGuardians.Remove(guardian);

                continue;
            }

            guardian.WeekNumber++;
            this.dbcontext.ChallengeGuardians.Update(guardian);
        }

        this.dbcontext.ChallengeGuardians.Add(
                                              new ChallengeGuardian
                                              {
                                                  Name = guardianName,
                                                  WeekNumber = 1,
                                              });

        await this.dbcontext.SaveChangesAsync();

        List<LfgModel> lfgModels = Program.StaticObjects.LfgModels;
        LfgModel oldModel = lfgModels.Find(x => x.MenuId.Contains("home-lfg") && x.MenuItemId == "challengeguardian");

        List<ChallengeGuardian> challengeGuardians = this.dbcontext.ChallengeGuardians.OrderByDescending(x => x.WeekNumber).ToList();

        oldModel.MenuBuilderOptions = new List<MenuBuilderOption>
        {
            new(challengeGuardians[0].Name, challengeGuardians[0].Name),
            new(challengeGuardians[1].Name, challengeGuardians[1].Name),
            new(challengeGuardians[2].Name, challengeGuardians[2].Name),
            new("All 3 Guardians", "All 3 Guardians"),
        };

        lfgModels.Remove(oldModel);
        lfgModels.Add(oldModel);
        Program.StaticObjects.LfgModels = lfgModels;

        await this.FollowupAsync("Challenge Guardians updated!", ephemeral: true);
    }

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