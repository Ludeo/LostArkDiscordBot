using Discord.WebSocket;
using LostArkBot.Src.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    public class MenuHandlerClass
    {
        public static async Task MenuHandler(SocketMessageComponent component)
        {
            List<LfgModel> lfgModels = Program.StaticObjects.LfgModels;
            List<ManageUserModel> manageUserModels = Program.StaticObjects.ManageUserModels;

            if (lfgModels.Any(x => x.MenuId.Contains(component.Data.CustomId)))
            {
                LfgModel resultModel = lfgModels.Find(x => x.MenuId.Contains(component.Data.CustomId) && x.MenuItemId == component.Data.Values.First());

                if(resultModel == null)
                {
                    resultModel = lfgModels.Find(x => x.MenuId.Contains(component.Data.CustomId) && x.IsEnd == true);
                }

                await LfgHandler.LfgHandlerAsync(component, resultModel);
            } else if(manageUserModels.Any(x => x.MenuId == component.Data.CustomId))
            {
                ManageUserModel resultModel = manageUserModels.Find(x => x.MenuId == component.Data.CustomId);

                await ManageUserHandler.ManageUserHandlerAsync(component, resultModel);
            } else if (component.Data.CustomId == "update")
            {
                await SubscriptionsHandler.Update(component);
            }
        }
    }
}