using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects.LostMerchants;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class RefreshVotesButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("refresh:*,*")]
        public async Task RefreshVotes(string merchantId, long expiryDateUnix)
        {
            var embed = Context.Interaction.Message.Embeds.First();
            DateTimeOffset expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix);
            if (expiryDate.CompareTo(DateTimeOffset.Now) < 0)
            {
                await ExpiredAsync(embed);
                return;
            }


            List<MerchantVote> activeMerchantVotes = await JsonParsers.GetActiveMerchantVotesJsonAsync();
            var merchantToRefresh = activeMerchantVotes.Find(merchant =>
            {
                return merchant.Id == merchantId;
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
            await Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = embed.ToEmbedBuilder().WithDescription("**Expired**").WithColor(Color.Red).Build();
            });
        }
    }
}