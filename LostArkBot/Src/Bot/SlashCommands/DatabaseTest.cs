using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [DontAutoRegister]
    public class DatabaseTest : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("databasetest", "x")]
        public async Task Test()
        {
            using(LostArkBotContext modelContext = new())
            {
                Character test = modelContext.Characters.Where(x => x.CharacterName == "Xludeo").First();

                await RespondAsync(test.CharacterName + ": " + test.ItemLevel);
            }
        }
    }
}