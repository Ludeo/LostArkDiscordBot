using Discord;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Models;
using LostArkBot.Src.Bot.Models.Enums;
using LostArkBot.Src.Bot.SlashCommands;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    public class SubscriptionsHandler
    {

        public static async Task Subscribe(SocketMessageComponent component)
        {
            WanderingMerchantItemsEnum selectedItem = (WanderingMerchantItemsEnum)Enum.Parse(typeof(WanderingMerchantItemsEnum), component.Data.Values.First());

            SocketUser user = component.User;

            string json;

            List<UserSubscription> merchantSubs;
            try
            {
                json = File.ReadAllText("MerchantSubscriptions.json");
            }
            catch (FileNotFoundException)
            {
                File.WriteAllText("MerchantSubscriptions.json", "[]");
                json = "[]";
            }

            merchantSubs = JsonSerializer.Deserialize<List<UserSubscription>>(json);

            UserSubscription subscription = merchantSubs.Find(sub =>
            {
                return sub.UserId == user.Id;
            });

            if (component.Data.CustomId == "subscribe")
            {
                if (subscription == null)
                {
                    List<int> newItems = new()
                    {
                        (int)selectedItem
                    };
                    var newSub = new UserSubscription(user.Id, newItems);

                    merchantSubs.Add(newSub);
                }
                else
                {
                    if (!subscription.SubscribedItems.Contains((int)selectedItem))
                    {
                        subscription.SubscribedItems.Add((int)selectedItem);
                    }
                    else
                    {
                        await component.RespondAsync($"{selectedItem} already added", ephemeral: true);
                        return;
                    }
                }

                try
                {
                    await component.RespondAsync();
                }
                catch (HttpException)
                {
                }

                await component.Message.DeleteAsync();

                File.WriteAllText("MerchantSubscriptions.json", JsonSerializer.Serialize(merchantSubs));
                await new SubscriptionsModule().SubscribeMenuBuilder(component.User);
            }

            if (component.Data.CustomId == "unsubscribe")
            {
                if (merchantSubs.Count == 0 || subscription == null)
                {
                    return;
                }

                if (!subscription.SubscribedItems.Contains((int)selectedItem))
                {
                    await component.RespondAsync($"{selectedItem} doesn't exist", ephemeral: true);
                    return;
                }

                subscription.SubscribedItems.Remove((int)selectedItem);

                try
                {
                    await component.RespondAsync();
                }
                catch (HttpException)
                {
                }

                await component.Message.DeleteAsync();

                File.WriteAllText("MerchantSubscriptions.json", JsonSerializer.Serialize(merchantSubs));

                await new SubscriptionsModule().UnsubscribeMenuBuilder(component.User);
            }

        }
    }
}
