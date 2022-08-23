using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Bot.Models.Enums;
using LostArkBot.Bot.SlashCommands;
using LostArkBot.databasemodels;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LostArkBot.Bot.Handlers;

public static class SubscriptionsHandler
{
    public static async Task Update(SocketMessageComponent component, LostArkBotContext dbcontext)
    {
        List<WanderingMerchantItemsEnum> newSubscribedItems = component.Data.Values
                                                                       .Select(
                                                                               x => (WanderingMerchantItemsEnum)Enum.Parse(
                                                                                typeof(WanderingMerchantItemsEnum),
                                                                                x)).ToList();

        List<int> parsedNewSubscribedItems = newSubscribedItems.Select(x => (int)x).ToList();

        User user = dbcontext.Users.FirstOrDefault(x => x.DiscordUserId == component.User.Id);

        if (user is null)
        {
            EntityEntry<User> userEntry = dbcontext.Users.Add(
                                                              new User
                                                              {
                                                                  DiscordUserId = component.User.Id,
                                                              });

            await dbcontext.SaveChangesAsync();
            user = userEntry.Entity;
        }

        List<int> allUserSubs = dbcontext.Subscriptions.Where(x => x.User == user).Select(x => x.ItemId).ToList();

        foreach (int item in parsedNewSubscribedItems.Where(item => !allUserSubs.Contains(item)))
        {
            dbcontext.Subscriptions.Add(
                                        new Subscription
                                        {
                                            ItemId = item,
                                            User = user,
                                        });
        }

        foreach (Subscription sub in from item in allUserSubs
                                     where !parsedNewSubscribedItems.Contains(item)
                                     select dbcontext.Subscriptions.First(x => x.UserId == user.Id && x.ItemId == item))
        {
            dbcontext.Subscriptions.Remove(sub);
        }

        await dbcontext.SaveChangesAsync();
        await component.Message.DeleteAsync();

        string activeSubs = await SubscriptionsModule.GetActiveSubscriptionsStringAsync(component.User.Id);
        IMessage message = await component.FollowupAsync("auto-delete");
        await message.DeleteAsync();
        await component.FollowupAsync($"Your current subscriptions:\n{activeSubs}", ephemeral: true);
    }
}