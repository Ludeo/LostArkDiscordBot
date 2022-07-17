using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Models;
using LostArkBot.Src.Bot.Models.Enums;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [Group("subscriptions", "Mange your merchant subscriptions")]
    public class SubscriptionsModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("list", "List of active merchant subscriptions")]
        public async Task Subscriptions()
        {
            SocketUser user = Context.User;
            string activeSubs = await GetActiveSubscriptionsStringAsync(user);
            await RespondAsync($"Your current subscriptions:\n{activeSubs}", ephemeral: true);
        }

        [SlashCommand("update", "Update subscription to selected merchant items")]
        public async Task Update()
        {
            SocketUser user = Context.User;
            await UpdateMenuBuilder(user);
            await RespondAsync("auto-delete");
            await DeleteOriginalResponseAsync();
        }

        public static async Task UpdateMenuBuilder(SocketUser user)
        {
            SelectMenuBuilder menu = new SelectMenuBuilder().WithCustomId("update").WithPlaceholder("Update subscription");
            UserSubscription userSub = await GetSubscriptionForUserAsync(user.Id);


            foreach (WanderingMerchantItemsEnum value in Enum.GetValues(typeof(WanderingMerchantItemsEnum)))
            {
                if (!userSub.SubscribedItems.Contains((int)value))
                {
                    menu = AddToMenu(menu, value, false);
                }

                if (userSub.SubscribedItems.Contains((int)value))
                {
                    menu = AddToMenu(menu, value, true);
                }
            }

            menu.WithMaxValues(menu.Options.Count);

            string activeSubs = await GetActiveSubscriptionsStringAsync(user);
            await BuildCommonComponentsAsync($"Your current subscriptions:\n{activeSubs}", user, false, menu);
        }

        private static async Task<UserSubscription> GetSubscriptionForUserAsync(ulong userId)
        {
            List<UserSubscription> merchantSubs = await JsonParsers.GetMerchantSubsFromJsonAsync();
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

        private static SelectMenuBuilder AddToMenu(SelectMenuBuilder menu, WanderingMerchantItemsEnum value, bool inList)
        {
            SelectMenuOptionBuilder option = new();
            string valueString = value.ToString();
            string labelString = value.ToString();

            if (inList)
            {
                option.WithDefault(true);
            }
            if (valueString == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.LegendaryRapport))
            {
                labelString = "Legendary Rapport";
            }

            option.WithLabel(labelString).WithValue(valueString);
            return menu.AddOption(option);
        }

        private static async Task BuildCommonComponentsAsync(string text, SocketUser user, bool withDelete = true, SelectMenuBuilder menu = null)
        {
            ComponentBuilder menuBuilder = new();

            if (menu != null)
            {
                menuBuilder = menuBuilder.WithSelectMenu(menu);
            }

            if (withDelete)
            {
                ButtonBuilder delete = Utils.DeepCopy(Program.StaticObjects.DeleteButton);
                delete.WithLabel("Exit");
                menuBuilder = menuBuilder.WithButton(delete);
            }

            MessageComponent menuComponent = menuBuilder.Build();
            await user.SendMessageAsync(text: text, components: menuComponent);
        }

        public static async Task<string> GetActiveSubscriptionsStringAsync(SocketUser user)
        {
            UserSubscription userSub = await GetSubscriptionForUserAsync(user.Id);

            string activeSubs = "";

            foreach (WanderingMerchantItemsEnum value in Enum.GetValues(typeof(WanderingMerchantItemsEnum)))
            {
                if (userSub.SubscribedItems.Contains((int)value))
                {
                    if (activeSubs != "")
                    {
                        activeSubs += "\n";
                    }

                    if (value.ToString() == "LegendaryRapport")
                    {
                        activeSubs += $" - Legendary Rapport";
                    }
                    else
                    {
                        activeSubs += $" - {value}";
                    }
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