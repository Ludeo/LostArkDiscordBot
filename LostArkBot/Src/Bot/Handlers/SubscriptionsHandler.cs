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

        public static async Task Subscribe(SocketMessageComponent component)
        {
            List<WanderingMerchantItemsEnum> selectedItems = component.Data.Values.Select(x =>
            {
                return (WanderingMerchantItemsEnum)Enum.Parse(typeof(WanderingMerchantItemsEnum), x);
            }).ToList();

            SocketUser user = component.User;

            List<UserSubscription> merchantSubs = await JsonParsers.GetMerchantSubsFromJsonAsync();

            UserSubscription subscription = merchantSubs.Find(sub =>
            {
                return sub.UserId == user.Id;
            });

            if (component.Data.CustomId == "subscribe")
            {
                if (subscription == null)
                {
                    List<int> newItems = selectedItems.Select(x => (int)x).ToList();
                    var newSub = new UserSubscription(user.Id, newItems);

                    merchantSubs.Add(newSub);
                }
                else
                {
                    selectedItems.ForEach(async selectedItem =>
                    {
                        if (subscription.SubscribedItems.Contains((int)selectedItem))
                        {
                            await component.RespondAsync($"{selectedItem} already added", ephemeral: true);
                            return;
                        }

                        subscription.SubscribedItems.Add((int)selectedItem);
                    });
                }

                await component.Message.DeleteAsync();

                await JsonParsers.WriteMerchantsAsync(merchantSubs);

                string activeSubs = await SubscriptionsModule.GetActiveSubscriptionsStringAsync(user);
                await component.RespondAsync($"Your current subscriptions:\n{activeSubs}", ephemeral: true);
            }

            if (component.Data.CustomId == "unsubscribe")
            {
                if (merchantSubs.Count == 0 || subscription == null)
                {
                    return;
                }

                selectedItems.ForEach(async selectedItem =>
                {
                    if (!subscription.SubscribedItems.Contains((int)selectedItem))
                    {
                        await component.RespondAsync($"{selectedItem} doesn't exist", ephemeral: true);
                        return;
                    }

                    subscription.SubscribedItems.Remove((int)selectedItem);
                });

                await component.Message.DeleteAsync();

                await JsonParsers.WriteMerchantsAsync(merchantSubs);

                string activeSubs = await SubscriptionsModule.GetActiveSubscriptionsStringAsync(user);
                await component.RespondAsync($"Your current subscriptions:\n{activeSubs}", ephemeral: true);
            }
        }
    }
}