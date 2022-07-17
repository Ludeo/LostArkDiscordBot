using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Models;
using LostArkBot.Src.Bot.Models.Enums;
using LostArkBot.Src.Bot.SlashCommands;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    public class SubscriptionsHandler
    {

        public static async Task Update(SocketMessageComponent component)
        {
            List<WanderingMerchantItemsEnum> newSubscribedItems = component.Data.Values.Select(x =>
            {
                return (WanderingMerchantItemsEnum)Enum.Parse(typeof(WanderingMerchantItemsEnum), x);
            }).ToList();

            SocketUser user = component.User;
            List<UserSubscription> allUserSubs = await JsonParsers.GetMerchantSubsFromJsonAsync();
            UserSubscription thisUserSubs = allUserSubs.Find(sub =>
            {
                return sub.UserId == user.Id;
            });


            List<int> parsedNewSubscribedItems = newSubscribedItems.Select(x => (int)x).ToList();

            if (thisUserSubs == null)
            {
                var newUserSub = new UserSubscription(user.Id, parsedNewSubscribedItems);

                allUserSubs.Add(newUserSub);
            }
            else
            {
                thisUserSubs.SubscribedItems = parsedNewSubscribedItems;
            }

            await component.Message.DeleteAsync();

            await JsonParsers.WriteMerchantsAsync(allUserSubs);

            string activeSubs = await SubscriptionsModule.GetActiveSubscriptionsStringAsync(user);
            await component.RespondAsync($"Your current subscriptions:\n{activeSubs}", ephemeral: true);

        }
    }
}