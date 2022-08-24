using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects.LostMerchants;
using LostArkBot.Bot.Shared;

namespace LostArkBot.Bot.Buttons;

public class RefreshVotesButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private async Task ExpiredAsync(IEmbed embed)
    {
        ComponentBuilder componentsBuilder = new();

        foreach (IMessageComponent component in this.Context.Interaction.Message.Components.First().Components)
        {
            ButtonComponent button = component as ButtonComponent;

            if (button.CustomId is not null && button.CustomId.Contains("refresh"))
            {
                componentsBuilder = componentsBuilder.WithButton(button.Label, button.CustomId, button.Style, button.Emote, button.Url, true);
            }
            else
            {
                componentsBuilder = componentsBuilder.WithButton(button.ToBuilder());
            }
        }

        await this.Context.Interaction.Message.ModifyAsync(x =>
        {
            x.Embed = embed.ToEmbedBuilder().WithDescription("**Expired or removed**")
                           .WithColor(Color.Red).Build();

            x.Components = componentsBuilder.Build();
        });
    }

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

        List<WebsiteMerchant> activeMerchants = await JsonParsers.GetActiveMerchantsJsonAsync();
        WebsiteMerchant merchantToRefresh = activeMerchants.Find(merchant => merchant.Id == merchantId);

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