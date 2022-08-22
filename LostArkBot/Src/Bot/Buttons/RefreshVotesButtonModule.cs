using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class RefreshVotesButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        private readonly LostArkBotContext dbcontext;

        public RefreshVotesButtonModule(LostArkBotContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        [ComponentInteraction("refresh:*,*")]
        public async Task RefreshVotes(string merchantId, long expiryDateUnix)
        {
            await DeferAsync();

            Embed embed = Context.Interaction.Message.Embeds.First();
            DateTimeOffset expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix);

            if (expiryDate.CompareTo(DateTimeOffset.Now) < 0)
            {
                await ExpiredAsync(embed);
                return;
            }

            List<ActiveMerchant> activeMerchants = dbcontext.ActiveMerchants.ToList();
            var merchantToRefresh = activeMerchants.Find(merchant =>
            {
                return merchant.MerchantId == merchantId;
            });

            if (merchantToRefresh == null)
            {
                await ExpiredAsync(embed);
                return;
            }

            EmbedFooterBuilder Footer = new()
            {
                Text = "Votes: " + merchantToRefresh.Votes.ToString(),
            };

            await Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = embed.ToEmbedBuilder().WithFooter(Footer).Build();
            });
        }

        public async Task ExpiredAsync(Embed embed)
        {
            ActionRowComponent component = Context.Interaction.Message.Components.First();

            await Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = embed.ToEmbedBuilder().WithDescription("**Expired or removed**").WithColor(Color.Red).Build();
                // TODO: fix
                //ButtonComponent button = component.Components.ToList().Find(x => x.CustomId.Contains("refresh")) as ButtonComponent;
                //button = button.ToBuilder().WithDisabled(true).Build();
            });
        }
    }
}