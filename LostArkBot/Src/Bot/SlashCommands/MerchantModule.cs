﻿using Discord;
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
using Discord.Rest;
using System.Threading;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class MerchantModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private HubConnection hubConnection;
        private Dictionary<string, MerchantInfo> merchantInfo;
        private readonly Dictionary<string, string> ansiColors = new()
        {
                { "Normal", "[0m" },
                { "Legendary", "[2;33m" },
                { "Epic", "[2;35m" },
                { "Rare", "[2;34m" },
                { "Uncommon", "[2;32m" }
        };

        private readonly List<Tuple<int, string>> thumbnails = new()
        {
                new Tuple<int, string>((int)WanderingMerchantItemsEnum.Wei, "https://lostarkcodex.com/icons/card_legend_01_0.webp"),
                new Tuple<int, string>((int)WanderingMerchantItemsEnum.Seria, "https://lostarkcodex.com/icons/card_rare_02_1.webp"),
                new Tuple<int, string>((int)WanderingMerchantItemsEnum.Sian, "https://lostarkcodex.com/icons/card_rare_06_2.webp"),
                new Tuple<int, string>((int)WanderingMerchantItemsEnum.Mokamoka, "https://lostarkcodex.com/icons/card_epic_00_7.webp"),
                new Tuple<int, string>((int)WanderingMerchantItemsEnum.Madnick, "https://lostarkcodex.com/icons/card_epic_01_7.webp"),
                new Tuple<int, string>((int)WanderingMerchantItemsEnum.Kaysarr, "https://lostarkcodex.com/icons/card_epic_03_6.webp"),
                new Tuple<int, string>((int)WanderingMerchantItemsEnum.LegendaryRapport, "https://lostarkcodex.com/icons/use_5_167.webp")
        };

        private readonly int voteThreshold = -5;

        [SlashCommand("reconnect-merchant", "Re-connects to lostmerchants and starts posting merchant locations when available")]
        public async Task ReconnectMerchantChannel()
        {
            if (Context.User.Id != Config.Default.Admin)
            {
                await RespondAsync(text: "You don't have permission to use this command", ephemeral: true);
                return;
            }

            if (Config.Default.MerchantChannel == 0 || Program.MerchantChannel == null)
            {
                await RespondAsync(text: "Merchant channel not set. Use /set-as-merchant-channel first", ephemeral: true);
                return;
            }

            if (Context.Channel.Id != Config.Default.MerchantChannel)
            {
                await RespondAsync(text: "This is not the merchant channel", ephemeral: true);
                return;
            }

            await StartMerchantChannel();
            await RespondAsync("Sucess", ephemeral: true);
        }

        public async Task StartMerchantChannel()
        {
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

            SocketTextChannel textChannel = Program.MerchantChannel;
            List<IMessage> messages = await textChannel.GetMessagesAsync().Flatten().ToListAsync();

            await textChannel.DeleteMessagesAsync(messages);
            DateTimeOffset now = DateTimeOffset.Now;
            DateTimeOffset nextMerchantsTime = now.AddHours(1).AddMinutes(-26).AddSeconds(-now.Second);

            await StartConnectionAsync();
            RestUserMessage msg = await Program.MerchantChannel.SendMessageAsync(text: "Merchant channel activated\nThis message will delete itself");
            Thread.Sleep(5 * 1000);
            await msg.DeleteAsync();

            await textChannel.SendMessageAsync($"Next merchants in: <t:{nextMerchantsTime.ToUnixTimeSeconds()}:R>");
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
            List<Merchant> jsonMerchants = await JsonParsers.GetActiveMerchantsJsonAsync();
            SocketTextChannel textChannel = Program.MerchantChannel;

            List<Merchant> activeMerchants;
            if (!triggeredManually)
            {
                MerchantGroup merchantGroup = JsonSerializer.Deserialize<MerchantGroup>(merchantGroupObj.ToString());
                activeMerchants = merchantGroup.ActiveMerchants;

                if (jsonMerchants.Count == 0)
                {
                    List<IMessage> messages = await textChannel.GetMessagesAsync().Flatten().ToListAsync();
                    await textChannel.DeleteMessagesAsync(messages);
                }
            }
            else
            {
                List<MerchantGroup> merchantGroups = JsonSerializer.Deserialize<List<MerchantGroup>>(merchantGroupObj.ToString());
                activeMerchants = merchantGroups.Select(x => x.ActiveMerchants.First()).ToList();

                Program.MerchantMessages = new();
                jsonMerchants = new();

                List<IMessage> messages = await textChannel.GetMessagesAsync().Flatten().ToListAsync();
                await textChannel.DeleteMessagesAsync(messages);
            }

            foreach (Merchant merchant in activeMerchants)
            {
                Merchant jsonStored = jsonMerchants.Find(x => x.Id == merchant.Id);
                if (jsonStored != null)
                {
                    continue;
                }

                await LogService.Log(LogSeverity.Debug, GetType().Name, $"Adding merchant: {merchant.Name}");

                int notableCard = -1;
                int notableRapport = -1;
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

                Embed embed = CreateMerchantEmbed(merchant, expiryDate, notableCard, notableRapport, MerchantEmbedTypeEnum.New).Build();

                jsonMerchants.Add(merchant);

                if (merchant.Votes <= voteThreshold)
                {
                    Program.MerchantMessages.Add(new MerchantMessage(merchant.Id, null, true));
                    return;
                }

                IUserMessage message = await Program.MerchantChannel.SendMessageAsync(text: rolePing, embed: embed);
                Program.MerchantMessages.Add(new MerchantMessage(merchant.Id, message.Id));


                if (notableCard != -1 || notableRapport != -1)
                {
                    await GetUserSubsriptions(notableCard, notableRapport, merchant, expiryDate);
                }
            }

            if (triggeredManually && (jsonMerchants.Count != activeMerchants.Count))
            {
                await LogService.Log(LogSeverity.Warning, GetType().Name, $"Number of merchants doesn't match. Merchant group: {activeMerchants.Count}, stored JSON: {jsonMerchants.Count}");
            }

            await JsonParsers.WriteActiveMerchantsAsync(jsonMerchants);
        }

        private void OnUpdateVotes()
        {
            hubConnection.On<List<object>>("UpdateVotes", async (votes) =>
            {
                bool merchantsUpdated = false;
                List<Merchant> activeMerchants = await JsonParsers.GetActiveMerchantsJsonAsync();
                List<MerchantVote> positiveVotes = new();
                List<MerchantVote> negativeVotes = new();

                foreach (object obj in votes)
                {
                    MerchantVote vote = JsonSerializer.Deserialize<MerchantVote>(obj.ToString());
                    if (vote.Votes <= voteThreshold)
                    {
                        negativeVotes.Add(vote);
                    }
                    else
                    {
                        positiveVotes.Add(vote);
                    }
                }

                foreach (MerchantVote vote in negativeVotes)
                {
                    MerchantMessage merchantMessage = Program.MerchantMessages.Find(x => x.MerchantId == vote.Id);
                    if (merchantMessage == null)
                    {
                        await LogService.Log(LogSeverity.Debug, GetType().Name, "Could not find merchant message to modify. Triggering merchant update.");
                        object merchantGroupObj = await hubConnection.InvokeAsync<object>("GetKnownActiveMerchantGroups", "Wei");

                        await UpdateMerchantGroupHandler(merchantGroupObj, true);
                        merchantsUpdated = true;
                        return;
                    }

                    merchantMessage.IsDeleted = true;
                    merchantMessage.MessageId = null;
                    IUserMessage message = await Program.MerchantChannel.GetMessageAsync((ulong)merchantMessage.MessageId) as IUserMessage;
                    await message.DeleteAsync();
                    await LogService.Log(LogSeverity.Info, GetType().Name, $"Deleted post for merchant with id {vote.Id}: {vote.Votes} votes");
                    Merchant merchant = activeMerchants.Find(x => x.Id == vote.Id);
                    merchant.Votes = vote.Votes;
                }

                if (merchantsUpdated)
                {
                    return;
                }

                foreach (MerchantVote vote in positiveVotes)
                {
                    MerchantMessage merchantMessage = Program.MerchantMessages.Find(x => x.MerchantId == vote.Id);
                    if (merchantMessage == null)
                    {
                        await LogService.Log(LogSeverity.Debug, GetType().Name, "Could not find merchant message to modify. Triggering merchant update.");
                        object merchantGroupObj = await hubConnection.InvokeAsync<object>("GetKnownActiveMerchantGroups", "Wei");

                        await UpdateMerchantGroupHandler(merchantGroupObj, true);
                        return;
                    }

                    IUserMessage message = await Program.MerchantChannel.GetMessageAsync((ulong)merchantMessage.MessageId) as IUserMessage;
                    IEmbed oldEmbed = message.Embeds.First();

                    EmbedBuilder newEmbed = oldEmbed.ToEmbedBuilder();
                    newEmbed.WithFooter($"Votes: {vote.Votes}");

                    await message.ModifyAsync(x => x.Embed = newEmbed.Build());

                    Merchant merchant = activeMerchants.Find(x => x.Id == vote.Id);
                    if (merchant == null)
                    {
                        await LogService.Log(LogSeverity.Warning, GetType().Name, $"Could not find active merchant with id {vote.Id}, This should no not happen normally");
                        continue;
                    }

                    merchant.Votes = vote.Votes;
                }

                await JsonParsers.WriteActiveMerchantsAsync(activeMerchants);
            });
        }

        private async Task<Task> GetUserSubsriptions(int notableCard, int notableRapport, Merchant merchant, DateTimeOffset expiryDate)
        {
            string url = "https://lostmerchants.com/";
            ButtonBuilder linkButton = new ButtonBuilder().WithLabel("Site").WithStyle(ButtonStyle.Link).WithUrl(url);
            ButtonBuilder refreshButton = new ButtonBuilder().WithCustomId($"refresh:{merchant.Id},{expiryDate.ToUnixTimeSeconds()}").WithEmote(new Emoji("\U0001F504")).WithStyle(ButtonStyle.Primary);
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

            await LogService.Log(LogSeverity.Debug, GetType().Name, GetNotableLogMsg(filteredSubscriptions.Count, notableCard, notableRapport));

            Embed embed = CreateMerchantEmbed(merchant, expiryDate, notableCard, notableRapport, MerchantEmbedTypeEnum.Subscription).Build();

            filteredSubscriptions.ForEach(async sub =>
            {
                SocketGuildUser serverUser = Context.Guild.GetUser(sub.UserId);

                if (serverUser == null)
                {
                    await LogService.Log(LogSeverity.Debug, GetType().Name, $"Server user with Id:{sub.UserId} not found with GetUser - trying with GetUserAsync");
                    ITextChannel interactionChannel = (ITextChannel)Context.Channel;
                    IGuildUser playerUser = await interactionChannel.Guild.GetUserAsync(sub.UserId);
                    serverUser = (SocketGuildUser)playerUser;

                    if (serverUser == null)
                    {
                        await LogService.Log(LogSeverity.Debug, GetType().Name, $"Server user not found with GetUserAsync");
                        return;
                    }
                    else
                    {
                        await LogService.Log(LogSeverity.Debug, GetType().Name, $"Server user found with GetUserAsync");
                    }
                }

                try
                {
                    await serverUser.SendMessageAsync(embed: embed, components: component);
                    await LogService.Log(LogSeverity.Info, GetType().Name, $"DM sent to {serverUser.Username}");
                }
                catch (HttpException e)
                {
                    await LogService.Log(LogSeverity.Error, GetType().Name, "User cannot receive DMs", e);
                }
                catch (Exception e)
                {
                    await LogService.Log(LogSeverity.Error, GetType().Name, "Error with SendMessageAsync", e);
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

        public EmbedBuilder CreateMerchantEmbed(Merchant merchant, DateTimeOffset expiryDate, int notableCard, int notableRapport, MerchantEmbedTypeEnum type)
        {

            if (type == MerchantEmbedTypeEnum.Debug)
            {
                merchantInfo = new() {
                    { "Test Merchant", new MerchantInfo()
                        {
                            Region="Test Region"
                        }
                    }
                };
            }

            EmbedBuilder embedBuilder = new();

            string merchantZoneUpdated = merchant.Zone.Replace(" ", "%20");

            embedBuilder.WithTitle(merchantInfo[merchant.Name].Region + " - " + merchant.Zone);
            embedBuilder.WithDescription($"Expires <t:{expiryDate.ToUnixTimeSeconds()}:R>");

            if (type == MerchantEmbedTypeEnum.New)
            {
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

                embedBuilder.WithThumbnailUrl("https://lostmerchants.com/images/zones/" + merchantZoneUpdated + ".jpg");
                embedBuilder.WithColor(embedColor);
            }

            if (type == MerchantEmbedTypeEnum.Subscription || type == MerchantEmbedTypeEnum.Debug)
            {
                if (notableCard != -1)
                {
                    embedBuilder.WithThumbnailUrl(thumbnails.Find(x => x.Item1 == notableCard).Item2);
                }
                else
                {
                    embedBuilder.WithThumbnailUrl(thumbnails.Find(x => x.Item1 == notableRapport).Item2);
                }

                embedBuilder.WithColor(Color.Green);
                embedBuilder.WithImageUrl("https://lostmerchants.com/images/zones/" + merchantZoneUpdated + ".jpg");
            }

            EmbedFieldBuilder cardField = new()
            {
                Name = ":black_joker: Card",
                Value = "```ansi\n" + ansiColors[merchant.Card.Rarity.ToString()] + merchant.Card.Name + "```",
                IsInline = true,
            };

            EmbedFieldBuilder rapportField = new()
            {
                Name = ":gift: Rapport",
                Value = "```ansi\n" + ansiColors[merchant.Rapport.Rarity.ToString()] + merchant.Rapport.Name + "```",
                IsInline = true,
            };

            embedBuilder.AddField(cardField);
            embedBuilder.AddField(rapportField);
            embedBuilder.WithFooter($"Votes: {merchant.Votes}");

            return embedBuilder;
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

        private static string GetNotableLogMsg(int filteredSubscriptionsCount, int notableCard, int notableRapport)
        {
            string logMsg = $"Found {filteredSubscriptionsCount} players subscribed to ";
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
            return logMsg;
        }
    }
}