using Discord;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Models;
using LostArkBot.Src.Bot.Models.Enums;
using LostArkBot.Src.Bot.Utils;
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


            WanderingMerchantGoodsEnum selectedItem = (WanderingMerchantGoodsEnum)Enum.Parse(typeof(WanderingMerchantGoodsEnum), component.Data.Values.First());

            SocketUser user = component.User;

            string json;

            List<UserSubscriptions> merchantSubs;
            try
            {
                json = File.ReadAllText("MerchantSubscriptions.json");
            }
            catch (FileNotFoundException)
            {
                File.WriteAllText("MerchantSubscriptions.json", "[]");
                return;
            }

            merchantSubs = JsonSerializer.Deserialize<List<UserSubscriptions>>(json);

            UserSubscriptions subscription = merchantSubs.Find(sub =>
            {
                return sub.UserId == user.Id;
            });

            if (component.Data.CustomId == "subscribe")
            {
                if (subscription == null)
                {
                    List<int> newItems = new List<int>
                    {
                        (int)selectedItem
                    };
                    var newSub = new UserSubscriptions(user.Id, newItems);

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
                        await component.RespondAsync($"{selectedItem.ToString()} already added");
                    }
                }
            }
            else
            {
                //TO-DO
                if (merchantSubs.Count == 0)
                {
                    return;
                }
            }

            File.WriteAllText("MerchantSubscriptions.json", JsonSerializer.Serialize(merchantSubs));

            try
            {
                await component.RespondAsync($"{selectedItem.ToString()} added");
            }
            catch (HttpException exception)
            {
                await LogService.Log(new LogMessage(LogSeverity.Error, "JoinCharacterMenu.cs", exception.Message));
            }

        }
    }
}
