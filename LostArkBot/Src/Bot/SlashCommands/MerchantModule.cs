using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.FileObjects.LostMerchants;
using LostArkBot.Src.Bot.Models;
using LostArkBot.Src.Bot.Shared;
using LostArkBot.Src.Bot.FileObjects.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using LostArkBot.Src.Bot.Models.Enums;
using System.Net.Http;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class MerchantModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private HubConnection hubConnection;
        private readonly SocketTextChannel merchantChannel = Program.MerchantChannel;
        private Dictionary<string, MerchantInfo> merchantInfo;
        private readonly Dictionary<string, string> ansiColors = new()
        {
                { "Normal", "[0m" },
                { "Legendary", "[2;33m" },
                { "Epic", "[2;35m" },
                { "Rare", "[2;34m" },
                { "Uncommon", "[2;32m" }
        };

        [SlashCommand("merchant", "Connects to lostmerchants and starts posting merchant locations when available")]
        public async Task Merchant()
        {
            if (Context.User.Id != Config.Default.Admin)
            {
                await RespondAsync(text: "You don't have permission to use this command", ephemeral: true);
                return;
            }

            await RespondAsync(text: "merchants activated", ephemeral: true);

            //HttpResponseMessage response = await new HttpClient().GetAsync("https://lostmerchants.com/data/merchants.json");
            //HttpContent content = response.Content;
            //string merchantInfoString = await content.ReadAsStringAsync();
            //merchantInfo = JsonSerializer.Deserialize<Dictionary<string, MerchantInfo>>(merchantInfoString);

            string merchantInfoString = new WebClient().DownloadString("https://lostmerchants.com/data/merchants.json");
            merchantInfo = JsonSerializer.Deserialize<Dictionary<string, MerchantInfo>>(merchantInfoString);

            //hubConnection = new HubConnectionBuilder()
            //    .WithUrl("https://test.lostmerchants.com/MerchantHub")
            //    .ConfigureLogging(logging =>
            //    {
            //        logging.SetMinimumLevel(LogLevel.Debug);
            //        logging.AddProvider(new SignalLoggerProvider());
            //    })
            //    .Build();

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
        }

        private void OnUpdateMerchantGroup()
        {
            hubConnection.On<string, object>("UpdateMerchantGroup", async (server, merchantGroupObj) =>
            {
                await UpdateMerchantGroupHandler(merchantGroupObj);
            });

        }

        private async Task UpdateMerchantGroupHandler(object merchantGroupObj, bool triggeredManually = false)
        {
            List<Merchant> activeMerchants;
            if (!triggeredManually)
            {
                MerchantGroup merchantGroup = JsonSerializer.Deserialize<MerchantGroup>(merchantGroupObj.ToString());
                activeMerchants = merchantGroup.ActiveMerchants;
            }
            else
            {
                List<MerchantGroup> merchantGroups = JsonSerializer.Deserialize<List<MerchantGroup>>(merchantGroupObj.ToString());
                activeMerchants = new List<Merchant>();
                merchantGroups.ForEach(mg =>
                {
                    activeMerchants.Add(mg.ActiveMerchants.First());
                });
            }

            List<Merchant> recordedMerchants = await JsonParsers.GetActiveMerchantsJsonAsync();

            if (recordedMerchants.Count == 0)
            {
                SocketTextChannel textChannel = Program.MerchantChannel;
                List<IMessage> messages = await textChannel.GetMessagesAsync().Flatten().ToListAsync();
                await textChannel.DeleteMessagesAsync(messages);
            }

            foreach (Merchant merchant in activeMerchants)
            {
                if (recordedMerchants.Find(x => x.Id == merchant.Id) != null)
                {
                    continue;
                }
                else
                {
                    recordedMerchants.Add(merchant);
                    await LogService.Log(LogSeverity.Debug, GetType().Name, $"Adding missing merchant: {merchant.Name}");

                }
                string merchantZoneUpdated = merchant.Zone.Replace(" ", "%20");
                int notableCard = -1;
                int notableRapport = -1;

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

                string rolePing = "";

                if (merchant.Card.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Wei))
                {
                    rolePing = "<@&986032976812982343> ";
                    notableCard = (int)WanderingMerchantItemsEnum.Wei;
                }
                else if (merchant.Card.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Mokamoka))
                {
                    rolePing = "<@&986033361770385429> ";
                    notableCard = (int)WanderingMerchantItemsEnum.Mokamoka;
                }
                else if (merchant.Card.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Sian))
                {
                    rolePing = "<@&986033048271331428> ";
                    notableCard = (int)WanderingMerchantItemsEnum.Sian;
                }
                else if (merchant.Card.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Seria))
                {
                    rolePing = "<@&986033604205371463> ";
                    notableCard = (int)WanderingMerchantItemsEnum.Seria;
                }
                else if (merchant.Card.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Madnick))
                {
                    rolePing = "<@&986033108954525836> ";
                    notableCard = (int)WanderingMerchantItemsEnum.Madnick;
                }
                else if (merchant.Card.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Kaysarr))
                {
                    rolePing = "<@&986033435531419679> ";
                    notableCard = (int)WanderingMerchantItemsEnum.Kaysarr;
                }

                if (merchant.Rapport.Rarity == Rarity.Legendary)
                {
                    rolePing += "<@&986032866053996554>";
                    notableRapport = (int)WanderingMerchantItemsEnum.LegendaryRapport;
                }

                if (string.IsNullOrEmpty(rolePing))
                {
                    rolePing = "\u200b";
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

                embedBuilder.WithFooter(new EmbedFooterBuilder()
                {
                    Text = $"Votes: {merchant.Votes}",
                });

                Embed embed = embedBuilder.Build();
                IUserMessage message = await merchantChannel.SendMessageAsync(text: rolePing, embed: embed);
                //await message.AddReactionAsync(new Emoji("✅"));
                Program.MerchantMessages.Add(new MerchantMessage(merchant.Id, message.Id));

                if (notableCard != -1 || notableRapport != -1)
                {
                    await GetUserSubsriptions(notableCard, notableRapport, embedBuilder, merchant.Id, expiryDate.ToUnixTimeSeconds());
                }


            }

            if (recordedMerchants.Count != activeMerchants.Count)
            {
                await LogService.Log(LogSeverity.Warning, GetType().Name, $"Number of merchants doesn't match. Merchant group: {activeMerchants.Count}, stored JSON: {recordedMerchants.Count}");
            }

            await JsonParsers.WriteActiveMerchantsAsync(recordedMerchants);
        }

        private void OnUpdateVotes()
        {
            hubConnection.On<List<object>>("UpdateVotes", async (votes) =>
            {
                List<MerchantVote> merchantVotes = new();
                List<Merchant> activeMerchants = await JsonParsers.GetActiveMerchantsJsonAsync();

                foreach (object obj in votes)
                {
                    MerchantVote vote = JsonSerializer.Deserialize<MerchantVote>(obj.ToString());
                    MerchantMessage merchantMessage = Program.MerchantMessages.Find(x => x.MerchantId == vote.Id);
                    if (merchantMessage == null)
                    {
                        await LogService.Log(LogSeverity.Debug, GetType().Name, $"Could not find merchant message to modify. Triggering merchant update.");
                        object merchantGroupObj = await hubConnection.InvokeAsync<object>($"GetKnownActiveMerchantGroups", "Wei");

                        await UpdateMerchantGroupHandler(merchantGroupObj, true);
                        return;
                    }

                    merchantVotes.Add(vote);
                    IUserMessage message = await merchantChannel.GetMessageAsync(merchantMessage.MessageId) as IUserMessage;
                    IEmbed oldEmbed = message.Embeds.First();

                    if (vote.Votes <= -5)
                    {
                        await message.DeleteAsync();
                        Program.MerchantMessages.Remove(merchantMessage);
                        return;
                    }

                    EmbedBuilder newEmbed = new()
                    {
                        Title = oldEmbed.Title,
                        Description = oldEmbed.Description,
                        ThumbnailUrl = oldEmbed.Thumbnail.Value.Url,
                        Color = oldEmbed.Color,
                        Footer = new EmbedFooterBuilder()
                        {
                            Text = $"Votes: {vote.Votes}"
                        }
                    };

                    foreach (EmbedField embedField in oldEmbed.Fields)
                    {
                        newEmbed.AddField(new EmbedFieldBuilder()
                        {
                            Name = embedField.Name,
                            Value = embedField.Value,
                            IsInline = embedField.Inline,
                        });
                    }

                    await message.ModifyAsync(x => x.Embed = newEmbed.Build());


                    Merchant merchant = activeMerchants.Find(x => x.Id == vote.Id);
                    if (merchant == null)
                    {
                        await LogService.Log(LogSeverity.Warning, GetType().Name, $"Could not find active merchant with id {vote.Id}, This should no not happen normally");
                        return;
                    }

                    merchant.Votes = vote.Votes;
                }

                await JsonParsers.WriteActiveMerchantsAsync(activeMerchants);
            });
        }

        private async Task<Task> GetUserSubsriptions(int notableCard, int notableRapport, EmbedBuilder embedBuilder, string merchantId, double expiryDate)
        {
            string url = "https://lostmerchants.com/";
            ButtonBuilder linkButton = new ButtonBuilder().WithLabel("Site").WithStyle(ButtonStyle.Link).WithUrl(url);
            ButtonBuilder refreshButton = new ButtonBuilder().WithCustomId($"refresh:{merchantId},{expiryDate}").WithEmote(new Emoji("\U0001F504")).WithStyle(ButtonStyle.Primary);
            MessageComponent component = new ComponentBuilder().WithButton(Program.StaticObjects.DeleteButton).WithButton(refreshButton).WithButton(linkButton).Build();

            List<UserSubscription> allSubscriptions = await JsonParsers.GetMerchantSubsFromJsonAsync();
            List<UserSubscription> filteredSubscriptions = allSubscriptions.FindAll(userSub =>
            {
                var card = userSub.SubscribedItems.Contains(notableCard);
                var rapport = userSub.SubscribedItems.Contains(notableRapport);

                return card || rapport;
            });

            if (allSubscriptions.Count == 0 || filteredSubscriptions.Count == 0)
            {
                return Task.CompletedTask;
            }

            string logMsg = $"Found {filteredSubscriptions.Count} players subscribed to ";
            if (notableCard != -1)
            {
                logMsg += (WanderingMerchantItemsEnum)notableCard;
            }
            if (notableRapport != -1)
            {
                if (notableCard != -1)
                {
                    logMsg += " and ";
                }
                logMsg += (WanderingMerchantItemsEnum)notableRapport;
            }
            await LogService.Log(LogSeverity.Debug, this.GetType().Name, logMsg);

            filteredSubscriptions.ForEach(async sub =>
            {
                SocketGuildUser serverUser = Context.Guild.GetUser(sub.UserId);

                if (serverUser == null)
                {
                    await LogService.Log(LogSeverity.Debug, this.GetType().Name, $"Server user with Id:{sub.UserId} not found");
                    return;
                }

                try
                {
                    await serverUser.SendMessageAsync(embed: embedBuilder.Build(), components: component);
                    await LogService.Log(LogSeverity.Info, this.GetType().Name, $"DM sent to {serverUser.Username}");
                }
                catch (HttpException exception)
                {
                    await LogService.Log(LogSeverity.Error, this.GetType().Name, "User cannot receive DMs", exception);
                }
            });

            return Task.CompletedTask;
        }

        private async Task StartConnectionAsync()
        {
            try
            {
                await hubConnection.StartAsync();
                hubConnection.Remove("SubscribeToServer");
                hubConnection.Remove("UpdateVotes");
                hubConnection.Remove("UpdateMerchantGroup");
                await hubConnection.InvokeAsync("SubscribeToServer", "Wei");
                OnUpdateMerchantGroup();
                OnUpdateVotes();
            }
            catch (Exception exception)
            {
                await OnConnectionExceptionAsync(exception);
            }
        }

        private async Task OnConnectionClosedAsync(Exception exception)
        {
            string errorMsg = exception != null ? exception.Message : "Connection error 'Merchants'";

            await LogService.Log(LogSeverity.Error, this.GetType().Name, errorMsg);
            await Task.Delay(2000);
            await StartConnectionAsync();
        }

        private async Task OnConnectionExceptionAsync(Exception exception)
        {
            await OnConnectionClosedAsync(exception);
        }
    }
}