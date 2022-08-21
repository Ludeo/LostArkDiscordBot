﻿using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [DontAutoRegister]
    public class DatabaseTest : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private LostArkBotContext context;

        public DatabaseTest(LostArkBotContext context)
        {
            this.context = context;
        }

        [SlashCommand("databasetest", "x")]
        public async Task Test()
        {
            Character test = context.Characters.Where(x => x.CharacterName == "Xludeo").First();

            await RespondAsync(test.CharacterName + ": " + test.ItemLevel);
        }
    }
}