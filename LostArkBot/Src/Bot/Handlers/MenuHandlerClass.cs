using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using LostArkBot.Bot.Models;
using LostArkBot.databasemodels;

namespace LostArkBot.Bot.Handlers;

public class MenuHandlerClass
{
    private readonly LostArkBotContext dbcontext;

    public MenuHandlerClass(LostArkBotContext dbcontext) => this.dbcontext = dbcontext;

    public async Task MenuHandler(SocketMessageComponent component)
    {
        await component.DeferAsync();

        List<LfgModel> lfgModels = Program.StaticObjects.LfgModels;
        List<ManageUserModel> manageUserModels = Program.StaticObjects.ManageUserModels;

        if (lfgModels.Any(x => x.MenuId.Contains(component.Data.CustomId)))
        {
            LfgModel resultModel = lfgModels.Find(x => x.MenuId.Contains(component.Data.CustomId) && x.MenuItemId == component.Data.Values.First())
                                ?? lfgModels.Find(x => x.MenuId.Contains(component.Data.CustomId) && x.IsEnd);

            await LfgHandler.LfgHandlerAsync(component, resultModel, this.dbcontext);
        }
        else if (manageUserModels.Any(x => x.MenuId == component.Data.CustomId))
        {
            ManageUserModel resultModel = manageUserModels.Find(x => x.MenuId == component.Data.CustomId);

            await ManageUserHandler.ManageUserHandlerAsync(component, resultModel, this.dbcontext);
        }
        else if (component.Data.CustomId == "update")
        {
            await SubscriptionsHandler.Update(component, this.dbcontext);
        }
    }
}