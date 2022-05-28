using Discord.WebSocket;
using LostArkBot.Bot.Modules;
using LostArkBot.Src.Bot.Modules;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    internal class SlashCommandHandlerClass
    {
        public static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "register":
                    await RegisterModule.RegisterModuleAsync(command);

                    break;

                case "update":
                    await UpdateModule.UpdateModuleAsync(command);

                    break;

                case "profile":
                    await ProfileModule.ProfileModuleAsync(command);

                    break;

                case "lfg":
                    await LfgModule.LfgModuleAsync(command);

                    break;

                case "account":
                    await AccountModule.AccountModuleAsync(command);

                    break;

                case "delete":
                    await DeleteModule.DeleteModuleAsync(command);

                    break;

                case "editmessage":
                    await EditMessageModule.EditMessageAsync(command);

                    break;

                case "serverstatus":
                    await ServerStatusModule.ServerStatusAsync(command);

                    break;

                case "help":
                    await HelpModule.HelpAsync(command);

                    break;

                case "roll":
                    await RollModule.RollAsync(command);

                    break;
            }
        }
    }
}