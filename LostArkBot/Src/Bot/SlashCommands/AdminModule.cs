using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
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
        [SlashCommand("edit-challenge-guardian", "Edits the list of current challenge guardians")]
        public async Task EditChallengeGuardian([Summary("name", "Name of the guardian")] string guardianName)
        {
            if (!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages || Context.Guild.Id != Config.Default.Server)
            {
                await RespondAsync(text: "You don't have permission to execute this command", ephemeral: true);
                return;
            }

            ChallengeNames challengeNames = await JsonParsers.GetChallengeNamesFromJson();
            string old = challengeNames.ChallengeGuardian.First();
            challengeNames.ChallengeGuardian.Remove(old);
            challengeNames.ChallengeGuardian.Add(guardianName);

            await JsonParsers.WriteChallengeNamesAsync(challengeNames);

            List<LfgModel> lfgModels = Program.StaticObjects.LfgModels;
            LfgModel oldModel = lfgModels.Find(x => x.MenuId.Contains("home-lfg") && x.MenuItemId == "challengeguardian");
            LfgModel newModel = oldModel;

            newModel.MenuBuilderOptions = new()
            {
                new MenuBuilderOption(challengeNames.ChallengeGuardian[0], challengeNames.ChallengeGuardian[0]),
                new MenuBuilderOption(challengeNames.ChallengeGuardian[1], challengeNames.ChallengeGuardian[1]),
                new MenuBuilderOption(challengeNames.ChallengeGuardian[2], challengeNames.ChallengeGuardian[2]),
            };

            lfgModels.Remove(oldModel);
            lfgModels.Add(newModel);
            Program.StaticObjects.LfgModels = lfgModels;

            await RespondAsync(text: "Challenge Guardians updated!", ephemeral: true);
        }

        [SlashCommand("edit-challenge-abyss", "Edits the list of current challenge abyss dungeons")]
        public async Task EditChallengeAbyss(
            [Summary("first", "Name of the first abyss dungeon")] string firstAbyssName,
            [Summary("second", "Name of the second abyss dungeon")] string secondAbyssName)
        {
            if (!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages || Context.Guild.Id != Config.Default.Server)
            {
                await RespondAsync(text: "You don't have permission to execute this command", ephemeral: true);
                return;
            }

            ChallengeNames challengeNames = await JsonParsers.GetChallengeNamesFromJson();

            List<string> newAbyss = new()
            {
                firstAbyssName,
                secondAbyssName,
            };
            challengeNames.ChallengeAbyss = newAbyss;

            await JsonParsers.WriteChallengeNamesAsync(challengeNames);

            List<LfgModel> lfgModels = Program.StaticObjects.LfgModels;
            LfgModel oldModel = lfgModels.Find(x => x.MenuId.Contains("home-lfg") && x.MenuItemId == "challengeabyss");
            LfgModel newModel = oldModel;

            newModel.MenuBuilderOptions = new()
            {
                new MenuBuilderOption(challengeNames.ChallengeAbyss[0], challengeNames.ChallengeAbyss[0]),
                new MenuBuilderOption(challengeNames.ChallengeAbyss[1], challengeNames.ChallengeAbyss[1]),
            };

            lfgModels.Remove(oldModel);
            lfgModels.Add(newModel);
            Program.StaticObjects.LfgModels = lfgModels;

            await RespondAsync(text: "Challenge Abyss updated!", ephemeral: true);
        }

        [SlashCommand("pick-random-member", "Randomly picks one of the guildmates")]
        public async Task PickRandomMember()
        {
            if (!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages || Context.Guild.Id != Config.Default.Server)
            {
                await RespondAsync(text: "You don't have permission to execute this command", ephemeral: true);
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

            await RespondAsync(text: $"Winner out of {guildMembers.Count} guild members: {winner.Mention}\n**Congratulations!!!!!!!!!!!!!**");
            await Context.Channel.SendMessageAsync(text: $"<@&{roleId}>");
        }
    }
}