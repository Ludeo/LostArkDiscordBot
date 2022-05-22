using Discord.WebSocket;
using System.Threading.Tasks;
using LostArkBot.Src.Bot.Buttons;

namespace LostArkBot.Src.Bot.Handlers
{
    internal class ButtonHandlerClass
    {
        public static async Task ButtonHandler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "delete":
                    await DeleteButton.Delete(component);

                    break;

                case "home":
                    await HomeButton.Home(component);

                    break;

                case "join":
                    await JoinButton.Join(component);

                    break;

                case "leave":
                    await LeaveButton.Leave(component);

                    break;

                case "start":
                    await StartButton.Start(component);

                    break;

                case "kick":
                    await KickButton.Kick(component);

                    break;
            }
        }
    }
}