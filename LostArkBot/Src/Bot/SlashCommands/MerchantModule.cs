using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.FileObjects.LostMerchants;
using LostArkBot.Src.Bot.Utils;
using LostArkBot.Src.Bot.FileObjects.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class MerchantModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        HubConnection hubConnection;

        [SlashCommand("merchant", "Connects to lostmerchants and starts posting merchant locations when available")]
        public async Task Merchant()
        {
            if (Context.User.Id != Config.Default.Admin)
            {
                await RespondAsync(text: "You don't have permission to use this command", ephemeral: true);
                return;
            }

            ITextChannel merchantChannel = Context.Channel as ITextChannel;

            await RespondAsync(text: "merchants activated", ephemeral: true);

            Dictionary<string, string> ansiColors = new()
            {
                { "Normal", "[0m" },
                { "Legendary", "[2;33m" },
                { "Epic", "[2;35m" },
                { "Rare", "[2;34m" },
                { "Uncommon", "[2;32m" }
            };

            string merchantInfoString = new WebClient().DownloadString("https://lostmerchants.com/data/merchants.json");
            Dictionary<string, MerchantInfo> merchantInfo = JsonSerializer.Deserialize<Dictionary<string, MerchantInfo>>(merchantInfoString);

            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://lostmerchants.com/MerchantHub")
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddProvider(new SignalLoggerProvider());
                })
                .Build();
            hubConnection.KeepAliveInterval = new TimeSpan(0, 4, 0);
            hubConnection.ServerTimeout = new TimeSpan(0, 8, 0);
            hubConnection.Closed -= OnConnectionClosedAsync;
            hubConnection.Closed += OnConnectionClosedAsync;

            

            await StartConnectionAsync();
            await hubConnection.InvokeAsync("SubscribeToServer", "Wei");

            hubConnection.On<string, object>("UpdateMerchantGroup", async (server, merchants) =>
            {
                MerchantGroup merchantGroup = JsonSerializer.Deserialize<MerchantGroup>(merchants.ToString());
                Merchant merchant = merchantGroup.ActiveMerchants.Last();
                string merchantZoneUpdated = merchant.Zone.Replace(" ", "%20");

                Rarity highestRarity = merchant.Card.Rarity;

                if (merchant.Rapport.Rarity > highestRarity)
                {
                    highestRarity = merchant.Rapport.Rarity;
                }

                Color embedColor = Color.Green;

                if (highestRarity == Rarity.Legendary)
                {
                    embedColor = Color.Gold;
                }
                else if (highestRarity == Rarity.Epic)
                {
                    embedColor = Color.Purple;
                }
                else if (highestRarity == Rarity.Rare)
                {
                    embedColor = Color.Blue;
                }

                string embedDescription = "";

                if (merchant.Card.Name == "Wei")
                {
                    embedDescription = "<@&986032976812982343> ";
                }
                else if (merchant.Card.Name == "Mokamoka")
                {
                    embedDescription = "<@&986033361770385429> ";
                }
                else if (merchant.Card.Name == "Sian")
                {
                    embedDescription = "<@&986033048271331428> ";
                }
                else if (merchant.Card.Name == "Seria")
                {
                    embedDescription = "<@&986033604205371463> ";
                }
                else if (merchant.Card.Name == "Madnick")
                {
                    embedDescription = "<@&986033108954525836> ";
                }
                else if (merchant.Card.Name == "Kaysarr")
                {
                    embedDescription = "<@&986033435531419679> ";
                }

                if (merchant.Rapport.Rarity == Rarity.Legendary)
                {
                    embedDescription += "<@&986032866053996554>";
                }

                if (string.IsNullOrEmpty(embedDescription))
                {
                    embedDescription = "\u200b";
                }

                DateTimeOffset now = DateTimeOffset.Now;
                DateTimeOffset expiryDate = new(now.Year, now.Month, now.Day, now.Hour, 55, 0, now.Offset);

                EmbedBuilder embedBuilder = new()
                {
                    Title = merchantInfo[merchant.Name].Region + " - " + merchant.Zone,
                    Description = $"Expires <t:{expiryDate.ToUnixTimeSeconds()}:R>",
                    ThumbnailUrl = "https://lostmerchants.com/images/zones/" + merchantZoneUpdated + ".jpg",
                    Color = embedColor,
                };

                embedBuilder.AddField(new EmbedFieldBuilder()
                {
                    Name = ":black_joker: Card",
                    Value = "```ansi\n" + ansiColors[merchant.Card.Rarity.ToString()] + merchant.Card.Name + "```",
                    IsInline = true,
                });

                embedBuilder.AddField(new EmbedFieldBuilder()
                {
                    Name = ":gift: Rapport",
                    Value = "```ansi\n" + ansiColors[merchant.Rapport.Rarity.ToString()] + merchant.Rapport.Name + "```",
                    IsInline = true,
                });

                Embed embed = embedBuilder.Build();
                IUserMessage message = await merchantChannel.SendMessageAsync(text: embedDescription, embed: embed);
                await message.AddReactionAsync(new Emoji("✅"));
            });

            hubConnection.On<List<object>>("UpdateVotes", async (votes) => {
                Console.WriteLine(votes.ToString());
                // create Object MerchantMessage - contains MessageId, MerchantId
                // on UpdateVotes get MerchantMessages that containd MerchantId
                // update the votes of the message which is linked to the merchant
                // if votes are negative, delete the message
            });
        }

        private async Task StartConnectionAsync()
        {
            try
            {
                await hubConnection.StartAsync();
            }
            catch (Exception exception)
            {
                await OnConnectionExceptionAsync(exception);
            }
        }

        private async Task OnConnectionClosedAsync(Exception exception)
        {
            string errorMsg = exception != null ? exception.Message : "Connection error 'Merchants'";

            await LogService.Log(new LogMessage(LogSeverity.Error, "MerchantModule.cs", errorMsg));
            await Task.Delay(2000);
            await StartConnectionAsync();
        }

        private async Task OnConnectionExceptionAsync(Exception exception)
        {
            await OnConnectionClosedAsync(exception);
        }
    }
}