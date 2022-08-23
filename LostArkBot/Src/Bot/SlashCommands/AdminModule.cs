using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Models;
using LostArkBot.Src.Bot.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [Group("admin", "Commands that only admins can use")]
    public class AdminModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly LostArkBotContext dbcontext;

        public AdminModule(LostArkBotContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [SlashCommand("edit-challenge-guardian", "Edits the list of current challenge guardians")]
        public async Task EditChallengeGuardian([Summary("name", "Name of the guardian")] string guardianName)
        {
            await DeferAsync(ephemeral: true);

            if (!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages || Context.Guild.Id != Config.Default.Server)
            {
                await FollowupAsync(text: "You don't have permission to execute this command", ephemeral: true);
                return;
            }

            foreach(ChallengeGuardian guardian in dbcontext.ChallengeGuardians.ToList())
            {
                if(guardian.WeekNumber == 3)
                {
                    dbcontext.ChallengeGuardians.Remove(guardian);
                    continue;
                }

                guardian.WeekNumber++;
                dbcontext.ChallengeGuardians.Update(guardian);
            }

            dbcontext.ChallengeGuardians.Add(new ChallengeGuardian { 
                Name = guardianName,
                WeekNumber = 1,
            });

            await dbcontext.SaveChangesAsync();

            List<LfgModel> lfgModels = Program.StaticObjects.LfgModels;
            LfgModel oldModel = lfgModels.Find(x => x.MenuId.Contains("home-lfg") && x.MenuItemId == "challengeguardian");
            LfgModel newModel = oldModel;

            List<ChallengeGuardian> challengeGuardians = dbcontext.ChallengeGuardians.OrderByDescending(x => x.WeekNumber).ToList();

            newModel.MenuBuilderOptions = new()
            {
                new MenuBuilderOption(challengeGuardians[0].Name, challengeGuardians[0].Name),
                new MenuBuilderOption(challengeGuardians[1].Name, challengeGuardians[1].Name),
                new MenuBuilderOption(challengeGuardians[2].Name, challengeGuardians[2].Name),
                new MenuBuilderOption("All 3 Guardians", "All 3 Guardians"),
            };

            lfgModels.Remove(oldModel);
            lfgModels.Add(newModel);
            Program.StaticObjects.LfgModels = lfgModels;

            await FollowupAsync(text: "Challenge Guardians updated!", ephemeral: true);
        }

        [SlashCommand("edit-challenge-abyss", "Edits the list of current challenge abyss dungeons")]
        public async Task EditChallengeAbyss(
            [Summary("first", "Name of the first abyss dungeon")] string firstAbyssName,
            [Summary("second", "Name of the second abyss dungeon")] string secondAbyssName)
        {
            await DeferAsync(ephemeral: true);

            if (!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages || Context.Guild.Id != Config.Default.Server)
            {
                await FollowupAsync(text: "You don't have permission to execute this command", ephemeral: true);
                return;
            }

            dbcontext.ChallengeAbysses.RemoveRange(dbcontext.ChallengeAbysses);
            dbcontext.ChallengeAbysses.Add(new ChallengeAbyss {
                Name = firstAbyssName,
            });
            dbcontext.ChallengeAbysses.Add(new ChallengeAbyss
            {
                Name = secondAbyssName,
            });

            await dbcontext.SaveChangesAsync();

            List<LfgModel> lfgModels = Program.StaticObjects.LfgModels;
            LfgModel oldModel = lfgModels.Find(x => x.MenuId.Contains("home-lfg") && x.MenuItemId == "challengeabyss");
            LfgModel newModel = oldModel;

            List<ChallengeAbyss> challengeAbysses = dbcontext.ChallengeAbysses.ToList();

            newModel.MenuBuilderOptions = new()
            {
                new MenuBuilderOption(challengeAbysses[0].Name, challengeAbysses[0].Name),
                new MenuBuilderOption(challengeAbysses[1].Name, challengeAbysses[1].Name),
                new MenuBuilderOption("Both Abysses", "Both Abysses"),
            };

            lfgModels.Remove(oldModel);
            lfgModels.Add(newModel);
            Program.StaticObjects.LfgModels = lfgModels;

            await FollowupAsync(text: "Challenge Abyss updated!", ephemeral: true);
        }

        [SlashCommand("pick-random-member", "Randomly picks one of the guildmates")]
        public async Task PickRandomMember()
        {
            await FollowupAsync();

            if (!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages || Context.Guild.Id != Config.Default.Server)
            {
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: "You don't have permission to execute this command", ephemeral: true);
                return;
            }

            List<IGuildUser> guildMembers = new();
            IEnumerable<IUser> users = await Context.Guild.GetUsersAsync().FlattenAsync();

            SocketRole role = Context.Guild.Roles.First(x => x.Name == "Guildmate");
            ulong roleId = role.Id;

            foreach (IGuildUser user in users)
            {
                if (user.RoleIds.FirstOrDefault(x => x == roleId) != 0)
                {
                    guildMembers.Add(user);
                }
            }

            IGuildUser winner = guildMembers[Program.Random.Next(guildMembers.Count)];

            await FollowupAsync(text: $"Winner out of {guildMembers.Count} guild members: {winner.Mention}\n**Congratulations!!!!!!!!!!!!!**");
            await Context.Channel.SendMessageAsync(text: $"<@&{roleId}>");
        }

        [SlashCommand("set-as-merchant-channel", "Select THIS channel to post merchant items")]
        public async Task SetMerchantChannel()
        {
            await DeferAsync(ephemeral: true);

            if (Context.User.Id != Config.Default.Admin)
            {
                await FollowupAsync(text: "You don't have permission to execute this command", ephemeral: true);
                return;
            }

            Config config = Config.Default;
            config.MerchantChannel = Context.Channel.Id;
            await JsonParsers.WriteConfigAsync(config);

            Program.MerchantChannel = Context.Channel as SocketTextChannel;
            Program.ReinitializeScheduledTasks();

            await FollowupAsync(text: $"Channel {Program.MerchantChannel.Name} is now a merchant channel", ephemeral: true);
        }
    }
}