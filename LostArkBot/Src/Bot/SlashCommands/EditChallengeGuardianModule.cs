using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
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
            if(!Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
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

            await RespondAsync(text: "Challenge Guardians updated!", ephemeral: true);
        }
    }
}