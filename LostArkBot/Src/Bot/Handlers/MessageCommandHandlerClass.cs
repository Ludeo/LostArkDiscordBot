using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    internal class MessageCommandHandlerClass
    {
        public static async Task MessageCommandHandler(SocketMessageCommand command)
        {
            Console.WriteLine("Message command received");
        }
    }
}
