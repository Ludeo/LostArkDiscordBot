using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects.LostMerchants;
using LostArkBot.Src.Bot.Models.Enums;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [DontAutoRegister]
    public class TestDMModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("testdm", "SendTestDM")]
        public async Task SendDM([Summary("item-id", "1-7")] int itemId)
        {

            if (itemId < 1 || itemId > 7)
            {
                await RespondAsync("Wrong item id: 1 to 7 only", ephemeral: true);
            }

            Merchant merchant = new()
            {
                Id = "12345",
                Name = "Test Merchant",
                Zone = "Leyar Terrace",
                Rapport = new MerchantItem(),
                Card = new MerchantItem(),
                Votes = 5,
            };
            merchant.Rapport.Name = "Test Rapport Item";
            merchant.Rapport.Rarity = Rarity.Epic;
            merchant.Card.Name = "Test Card";
            merchant.Card.Rarity = Rarity.Rare;

            int notableCard = -1;
            int notableRapport = -1;

            if (itemId == 7)
            {
                merchant.Rapport.Name = "Legendary Rapport";
                merchant.Rapport.Rarity = Rarity.Legendary;
                notableRapport = itemId;
            }
            else
            {
                merchant.Card.Name = Enum.GetName(typeof(WanderingMerchantItemsEnum), itemId);
                merchant.Card.Rarity = Rarity.Rare;
                notableCard = itemId;
            }

            DateTimeOffset now = DateTimeOffset.Now;
            DateTimeOffset expiryDate = now.AddMinutes(10);

            Embed embed = new MerchantModule().CreateMerchantEmbed(merchant, expiryDate, notableCard, notableRapport, MerchantEmbedTypeEnum.Debug).Build();

            string url = "https://lostmerchants.com/";
            ButtonBuilder linkButton = new ButtonBuilder().WithLabel("Site").WithStyle(ButtonStyle.Link).WithUrl(url);
            ButtonBuilder refreshButton = new ButtonBuilder().WithCustomId($"refresh:{merchant.Id},{expiryDate.ToUnixTimeSeconds()}").WithEmote(new Emoji("\U0001F504")).WithStyle(ButtonStyle.Primary);

            SocketGuildUser serverUser = Context.Guild.GetUser(Context.User.Id);
            MessageComponent component = new ComponentBuilder().WithButton(Program.StaticObjects.DeleteButton).WithButton(refreshButton).WithButton(linkButton).Build();

            await serverUser.SendMessageAsync(embed: embed, components: component);

            await RespondAsync("Done", ephemeral: true);
        }
    }
}