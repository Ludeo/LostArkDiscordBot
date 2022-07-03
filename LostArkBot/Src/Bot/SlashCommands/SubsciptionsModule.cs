using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Models;
using LostArkBot.Src.Bot.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class SubscriptionsModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("subscribe", "Get DM's for selected merchant items")]
        public async Task Subscribe()
        {
            SocketUser user = Context.User;
            await SubscribeMenuBuilder(user);
            await RespondAsync("auto-delete");
            await DeleteOriginalResponseAsync();
        }

        [SlashCommand("unsubscribe", "Remove subscription to selected merchant items")]
        public async Task Unsubscribe()
        {
            SocketUser user = Context.User;
            await UnsubscribeMenuBuilder(user);
            await RespondAsync("auto-delete");
            await DeleteOriginalResponseAsync();
        }

        [SlashCommand("subscriptions", "List of active merchant subscriptions")]
        public async Task Subscriptions()
        {
            SocketUser user = Context.User;
            string activeSubs = GetActiveSubscriptionsString(user);
            await RespondAsync($"Your current subscriptions:\n{activeSubs}", ephemeral: true);
        }

        public async Task SubscribeMenuBuilder(SocketUser user)
        {
            SelectMenuBuilder menu = new SelectMenuBuilder().WithCustomId("subscribe").WithPlaceholder("Add subscription");
            UserSubscription userSub = GetSubscriptionForUser(user.Id);

            if (userSub != null && userSub.SubscribedItems.Count == Enum.GetValues(typeof(WanderingMerchantItemsEnum)).Length)
            {
                await BuildCommonComponentsAsync("You are already subscribed to everything", user);
                return;
            }

            foreach (WanderingMerchantItemsEnum value in Enum.GetValues(typeof(WanderingMerchantItemsEnum)))
            {
                if (!userSub.SubscribedItems.Contains((int)value))
                {
                    menu = AddToMenu(menu, value);
                }
            }

            string activeSubs = GetActiveSubscriptionsString(user);
            await BuildCommonComponentsAsync($"Your current subscriptions:\n{activeSubs}", user, menu);
        }

        public async Task UnsubscribeMenuBuilder(SocketUser user)
        {
            SelectMenuBuilder menu = new SelectMenuBuilder().WithCustomId("unsubscribe").WithPlaceholder("Remove subscription");
            UserSubscription userSub = GetSubscriptionForUser(user.Id);

            if (userSub == null || userSub.SubscribedItems.Count == 0)
            {
                await BuildCommonComponentsAsync("You don't have any active subscriptions", user);
                return;
            }

            foreach (WanderingMerchantItemsEnum value in Enum.GetValues(typeof(WanderingMerchantItemsEnum)))
            {
                if (userSub.SubscribedItems.Contains((int)value))
                {
                    menu = AddToMenu(menu, value);
                }
            }
            string activeSubs = GetActiveSubscriptionsString(user);
            await BuildCommonComponentsAsync($"Your current subscriptions:\n{activeSubs}", user, menu);
        }

        private static UserSubscription GetSubscriptionForUser(ulong userId)
        {
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
            UserSubscription userSub = merchantSubs.Find(sub =>
            {
                return sub.UserId == userId;
            });

            if (userSub == null)
            {
                UserSubscription newSub = new(userId, new List<int>());
                return newSub;
            }

            return userSub;
        }

        private static SelectMenuBuilder AddToMenu(SelectMenuBuilder menu, WanderingMerchantItemsEnum value)
        {
            string valueString = value.ToString();
            string labelString = value.ToString();
            if (valueString == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.LegendaryRapport))
            {
                labelString = "Legendary Rapport";
            }
            return menu.AddOption(labelString, valueString);
        }

        private static async Task BuildCommonComponentsAsync(string text, SocketUser user, SelectMenuBuilder menu = null)
        {
            ButtonBuilder delete = Shared.Utils.DeepCopy(Program.StaticObjects.DeleteButton);
            delete.WithLabel("Exit");

            ComponentBuilder menuBuilder = new();

            if (menu != null)
            {
                menuBuilder = menuBuilder.WithSelectMenu(menu);
            }

            menuBuilder = menuBuilder.WithButton(delete);
            MessageComponent menuComponent = menuBuilder.Build();
            await user.SendMessageAsync(text: text, components: menuComponent);
        }

        private static string GetActiveSubscriptionsString(SocketUser user)
        {
            UserSubscription userSub = GetSubscriptionForUser(user.Id);

            string activeSubs = "";

            foreach (WanderingMerchantItemsEnum value in Enum.GetValues(typeof(WanderingMerchantItemsEnum)))
            {
                if (userSub.SubscribedItems.Contains((int)value))
                {
                    if (activeSubs != "") activeSubs += "\n";
                    activeSubs += $" - {value}";
                }
            }

            if (activeSubs == "")
            {
                activeSubs = " *No subscriptions*";
            }

            return activeSubs;
        }
    }
}