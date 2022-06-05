using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    internal class UserCommandHandlerClass
    {
        public static async Task UserCommandHandler(SocketUserCommand command)
        {
            Console.WriteLine("User command received");
        }
    }
}
