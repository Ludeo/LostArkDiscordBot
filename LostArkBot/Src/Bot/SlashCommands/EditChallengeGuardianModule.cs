using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class EditChallengeGuardianModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [RequireUserPermission(Discord.ChannelPermission.ManageChannels)]
        [SlashCommand("edit-challenge-guardian", "Edits the list of the current challenge guardians")]
        public async Task EditChallengeGuardian([Summary("name", "Name of the guardian")] string guardianName)
        {
            if (!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                await RespondAsync(text: "You don't have permission to execute this command", ephemeral: true);
                return;
            }

            List<ChallengeGuardian> challengeGuardians = JsonSerializer.Deserialize<List<ChallengeGuardian>>(File.ReadAllText("challengeguardians.json"));
            ChallengeGuardian old = challengeGuardians.First();
            challengeGuardians.Remove(old);

            ChallengeGuardian newChallengeGuardian = new()
            {
                GuardianName = guardianName,
            };

            challengeGuardians.Add(newChallengeGuardian);
            File.WriteAllText("challengeguardians.json", JsonSerializer.Serialize(challengeGuardians));

            List<LfgModel> lfgModels = Program.StaticObjects.LfgModels;
            LfgModel oldModel = lfgModels.Find(x => x.MenuId.Contains("home-lfg") && x.MenuItemId == "challengeguardian");
            LfgModel newModel = oldModel;
            newModel.MenuBuilderOptions = new()
            {
                new MenuBuilderOption(challengeGuardians[0].GuardianName, challengeGuardians[0].GuardianName),
                new MenuBuilderOption(challengeGuardians[1].GuardianName, challengeGuardians[1].GuardianName),
                new MenuBuilderOption(challengeGuardians[2].GuardianName, challengeGuardians[2].GuardianName),
            };
            lfgModels.Remove(oldModel);
            lfgModels.Add(newModel);
            Program.StaticObjects.LfgModels = lfgModels;

            await RespondAsync(text: "Challenge Guardians updated!", ephemeral: true);
        }
    }
}