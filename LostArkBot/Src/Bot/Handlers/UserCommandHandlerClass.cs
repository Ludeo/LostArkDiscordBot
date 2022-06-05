using Discord.WebSocket;
using LostArkBot.Src.Bot.Modules;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    internal class UserCommandHandlerClass
    {
        public static async Task UserCommandHandler(SocketUserCommand command)
        {
            Console.WriteLine("User command received");

            switch (command.Data.Name) {
                case "characters":

                    await AccountModule.AccountModuleAsync(command);
                    break;
            }
        }
    }
}
