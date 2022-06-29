using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class SubscribeModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("subscribe", "Get DM's for wandering merchant items")]
        public async Task Subscribe()
        {

            SelectMenuBuilder subscribeMenu = new SelectMenuBuilder().WithCustomId("subscribe").WithPlaceholder("Select Items");
            ulong userId = Context.User.Id;

            foreach (WanderingMerchantGoodsEnum value in Enum.GetValues(typeof(WanderingMerchantGoodsEnum))) {
                string valueString = value.ToString();
                subscribeMenu.AddOption(valueString, valueString);
            }

            await RespondAsync(text: "Select items to recieve DM's for", components: new ComponentBuilder().WithSelectMenu(subscribeMenu).Build(), ephemeral: true);
        }
    }
}
