using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using LostArkBot.Src.Bot.Models.Enums;
using LostArkBot.Src.Bot.Shared;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [Group("subscriptions", "Mange your merchant subscriptions")]
    public class SubscriptionsModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private static LostArkBotContext dbcontext;

        public SubscriptionsModule(LostArkBotContext _dbcontext)
        {
            dbcontext = _dbcontext;
        }

        [SlashCommand("list", "List of active merchant subscriptions")]
        public async Task Subscriptions()
        {
            await DeferAsync(ephemeral: true);
            string activeSubs = await GetActiveSubscriptionsStringAsync(Context.User.Id);
            await FollowupAsync($"Your current subscriptions:\n{activeSubs}", ephemeral: true);
        }

        [SlashCommand("update", "Update subscription to selected merchant items")]
        public async Task Update()
        {
            await DeferAsync();
            await UpdateMenuBuilder(Context.User);
            IMessage message = await FollowupAsync("auto-delete");
            await message.DeleteAsync();
        }

        public static async Task UpdateMenuBuilder(SocketUser user)
        {
            SelectMenuBuilder menu = new SelectMenuBuilder().WithCustomId("update").WithPlaceholder("Update subscription");
            List<int> userSub = await GetSubscriptionForUserAsync(user.Id);

            foreach (WanderingMerchantItemsEnum value in Enum.GetValues(typeof(WanderingMerchantItemsEnum)))
            {
                if (!userSub.Contains((int)value))
                {
                    menu = AddToMenu(menu, value, false);
                }

                if (userSub.Contains((int)value))
                {
                    menu = AddToMenu(menu, value, true);
                }
            }

            menu.WithMaxValues(menu.Options.Count);

            string activeSubs = await GetActiveSubscriptionsStringAsync(user.Id);
            await BuildCommonComponentsAsync($"Your current subscriptions:\n{activeSubs}", user, false, menu);
        }

        private static async Task<List<int>> GetSubscriptionForUserAsync(ulong userId)
        {
            User user = dbcontext.Users.Where(x => x.DiscordUserId == userId).FirstOrDefault();

            if (user is null)
            {
                EntityEntry<User> userEntry = dbcontext.Users.Add(new User
                {
                    DiscordUserId = userId,
                });

                await dbcontext.SaveChangesAsync();
            }

            List<int> userSub = dbcontext.Subscriptions.Where(x => x.User.DiscordUserId == userId).Select(x => x.ItemId).ToList();

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

        public static async Task<string> GetActiveSubscriptionsStringAsync(ulong userId)
        {
            List<int> userSub = await GetSubscriptionForUserAsync(userId);

            string activeSubs = "";

            foreach (WanderingMerchantItemsEnum value in Enum.GetValues(typeof(WanderingMerchantItemsEnum)))
            {
                if (userSub.Contains((int)value))
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