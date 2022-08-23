using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;

namespace LostArkBot.Bot.Buttons;

public class RefreshVotesButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly LostArkBotContext dbcontext;

    public RefreshVotesButtonModule(LostArkBotContext dbcontext) => this.dbcontext = dbcontext;

    private async Task ExpiredAsync(IEmbed embed) =>
        await this.Context.Interaction.Message.ModifyAsync(
                                                           x =>
                                                           {
                                                               x.Embed = embed.ToEmbedBuilder().WithDescription("**Expired or removed**")
                                                                              .WithColor(Color.Red).Build();
                                                               // TODO: fix
                                                               //ButtonComponent button = component.Components.ToList().Find(x => x.CustomId.Contains("refresh")) as ButtonComponent;
                                                               //button = button.ToBuilder().WithDisabled(true).Build();
                                                           });

    [ComponentInteraction("refresh:*,*")]
    public async Task RefreshVotes(string merchantId, long expiryDateUnix)
    {
        await this.DeferAsync();

        Embed embed = this.Context.Interaction.Message.Embeds.First();
        DateTimeOffset expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix);

        if (expiryDate.CompareTo(DateTimeOffset.Now) < 0)
        {
            await this.ExpiredAsync(embed);

            return;
        }

        List<ActiveMerchant> activeMerchants = this.dbcontext.ActiveMerchants.ToList();
        ActiveMerchant merchantToRefresh = activeMerchants.Find(merchant => merchant.Id == merchantId);

        if (merchantToRefresh == null)
        {
            await this.ExpiredAsync(embed);

            return;
        }

        EmbedFooterBuilder footer = new()
        {
            Text = "Votes: " + merchantToRefresh.Votes,
        };

        await this.Context.Interaction.Message.ModifyAsync(x => { x.Embed = embed.ToEmbedBuilder().WithFooter(footer).Build(); });
    }
}