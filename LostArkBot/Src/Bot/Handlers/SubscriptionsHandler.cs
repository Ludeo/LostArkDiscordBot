using Discord.WebSocket;
using LostArkBot.Src.Bot.Models.Enums;
using LostArkBot.Src.Bot.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using LostArkBot.databasemodels;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LostArkBot.Src.Bot.Handlers
{
    public class SubscriptionsHandler
    {
        public static async Task Update(SocketMessageComponent component, LostArkBotContext dbcontext)
        {
            List<WanderingMerchantItemsEnum> newSubscribedItems = component.Data.Values.Select(x =>
            {
                return (WanderingMerchantItemsEnum)Enum.Parse(typeof(WanderingMerchantItemsEnum), x);
            }).ToList();

            List<int> parsedNewSubscribedItems = newSubscribedItems.Select(x => (int)x).ToList();

            User user = dbcontext.Users.Where(x => x.DiscordUserId == component.User.Id).FirstOrDefault();

            if(user is null)
            {
                EntityEntry<User> userEntry = dbcontext.Users.Add(new User
                {
                    DiscordUserId = component.User.Id,
                });

                await dbcontext.SaveChangesAsync();
                user = userEntry.Entity;
            }

            List<int> allUserSubs = dbcontext.Subscriptions.Where(x => x.User == user).Select(x => x.ItemId).ToList();

            foreach(int item in parsedNewSubscribedItems)
            {
                if(allUserSubs.Contains(item))
                {
                    continue;
                }

                dbcontext.Subscriptions.Add(new Subscription
                {
                    ItemId = item,
                    User = user,
                });
            }

            await dbcontext.SaveChangesAsync();
            await component.Message.DeleteAsync();

            string activeSubs = await SubscriptionsModule.GetActiveSubscriptionsStringAsync(component.User.Id);
            IMessage message = await component.FollowupAsync("auto-delete");
            await message.DeleteAsync();
            await component.FollowupAsync($"Your current subscriptions:\n{activeSubs}", ephemeral: true);
        }
    }
}