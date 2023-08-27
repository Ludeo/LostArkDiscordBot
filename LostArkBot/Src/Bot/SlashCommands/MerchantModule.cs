using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using LostArkBot.Bot.FileObjects.LostMerchants;
using LostArkBot.Bot.FileObjects.SignalR;
using LostArkBot.Bot.Models.Enums;
using LostArkBot.Bot.Shared;
using LostArkBot.databasemodels;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using MerchantItem = LostArkBot.Bot.FileObjects.LostMerchants.MerchantItem;

namespace LostArkBot.Bot.SlashCommands;

public class MerchantModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    private const int VoteThreshold = -5;

    private readonly Dictionary<string, string> ansiColors = new()
    {
        { "Normal", "[0m" },
        { "Legendary", "[2;33m" },
        { "Epic", "[2;35m" },
        { "Rare", "[2;34m" },
        { "Uncommon", "[2;32m" },
    };

    private readonly LostArkBotContext dbcontext;
    private HubConnection hubConnection;
    private SocketTextChannel textChannel;
    private Dictionary<string, MerchantInfo> merchantInfo;

    public MerchantModule(LostArkBotContext dbcontext) => this.dbcontext = dbcontext;

    private EmbedBuilder CreateMerchantEmbed(
        WebsiteMerchant merchant,
        DateTimeOffset expiryDate,
        MerchantEmbedTypeEnum type)
    {
        if (type == MerchantEmbedTypeEnum.Debug)
        {
            this.merchantInfo = new Dictionary<string, MerchantInfo>
            {
                {
                    "Test Merchant", new MerchantInfo
                    {
                        Region = "Test Region",
                    }
                },
            };
        }

        EmbedBuilder embedBuilder = new();
        embedBuilder.WithTitle(merchant.Name + " (" + this.merchantInfo[merchant.Name].Region + ")");
        embedBuilder.WithDescription($"Expires <t:{expiryDate.ToUnixTimeSeconds()}:R>");

        Rarity highestRarity = Rarity.Uncommon;

        foreach (MerchantItem item in merchant.Cards)
        {
            if (item.Rarity > highestRarity)
            {
                highestRarity = item.Rarity;
            }
        }

        foreach (MerchantItem item in merchant.Rapports)
        {
            if (item.Rarity > highestRarity)
            {
                highestRarity = item.Rarity;
            }
        }

        Color embedColor = highestRarity switch
        {
            Rarity.Legendary => Color.Gold,
            Rarity.Epic      => Color.Purple,
            Rarity.Rare      => Color.Blue,
            _                => Color.Green,
        };

        embedBuilder.WithColor(embedColor);

        foreach (MerchantItem item in merchant.Cards)
        {
            EmbedFieldBuilder cardField = new()
            {
                Name = ":black_joker: Card",
                Value = "```ansi\n" + this.ansiColors[item.Rarity.ToString()] + item.Name + "```",
                IsInline = true,
            };

            embedBuilder.AddField(cardField);
        }

        foreach (MerchantItem item in merchant.Rapports)
        {
            EmbedFieldBuilder rapportField = new()
            {
                Name = ":gift: Rapport",
                Value = "```ansi\n" + this.ansiColors[item.Rarity.ToString()] + item.Name + "```",
                IsInline = true,
            };

            embedBuilder.AddField(rapportField);
        }

        embedBuilder.WithFooter($"Votes: {merchant.Votes}");

        return embedBuilder;
    }

    private static string GetNotableLogMsg(int filteredSubscriptionsCount, List<int> notableCard, List<int> notableRapport)
    {
        string logMsg = $"Found {filteredSubscriptionsCount} players subscribed to Cards: ";

        logMsg = notableCard.Aggregate(logMsg, (current, card) => current + ((WanderingMerchantItemsEnum)card + ", "));
        logMsg = logMsg[..^2];

        logMsg += " and Rapports: ";
        logMsg = notableRapport.Aggregate(logMsg, (current, rapport) => current + ((WanderingMerchantItemsEnum)rapport + ", "));
        logMsg = logMsg[..^2];

        return logMsg;
    }

    private async Task GetUserSubscriptions(List<int> notableCard, List<int> notableRapport, WebsiteMerchant merchant, DateTimeOffset expiryDate)
    {
        const string url = "https://lostmerchants.com/";
        ButtonBuilder linkButton = new ButtonBuilder().WithLabel("Site").WithStyle(ButtonStyle.Link).WithUrl(url);

        ButtonBuilder refreshButton = new ButtonBuilder().WithCustomId($"refresh:{merchant.Id},{expiryDate.ToUnixTimeSeconds()}")
                                                         .WithEmote(new Emoji("\U0001F504")).WithStyle(ButtonStyle.Primary);

        MessageComponent component = new ComponentBuilder().WithButton(Program.StaticObjects.DeleteButton).WithButton(refreshButton)
                                                           .WithButton(linkButton).Build();

        List<Subscription> subscriptions = this.dbcontext.Subscriptions.Where(x => notableCard.Any(y => y == x.ItemId) || notableRapport.Any(y => y == x.ItemId)).ToList();

        if (subscriptions.Count == 0)
        {
            return;
        }

        List<int> distinctUserIds = subscriptions.Select(x => x.UserId).Distinct().ToList();

        await LogService.Log(LogSeverity.Debug, this.GetType().Name, GetNotableLogMsg(distinctUserIds.Count, notableCard, notableRapport));

        Embed embed = this.CreateMerchantEmbed(merchant, expiryDate, MerchantEmbedTypeEnum.Subscription).Build();

        async void SendMessageToUser(int subUser)
        {
            User user = this.dbcontext.Users.FirstOrDefault(x => x.Id == subUser);
            SocketGuildUser serverUser = Program.Guild.GetUser(user.DiscordUserId);

            if (serverUser == null)
            {
                await LogService.Log(
                                     LogSeverity.Debug,
                                     this.GetType().Name,
                                     $"Server user with Id:{user.DiscordUserId} not found with GetUser - trying with GetUserAsync");

                ITextChannel interactionChannel = (ITextChannel)this.Context.Channel;
                IGuildUser playerUser = await interactionChannel.Guild.GetUserAsync(user.DiscordUserId);
                serverUser = (SocketGuildUser)playerUser;

                if (serverUser == null)
                {
                    await LogService.Log(LogSeverity.Debug, this.GetType().Name, "Server user not found with GetUserAsync");

                    return;
                }

                await LogService.Log(LogSeverity.Debug, this.GetType().Name, "Server user found with GetUserAsync");
            }

            try
            {
                await serverUser.SendMessageAsync(embed: embed, components: component);
                await LogService.Log(LogSeverity.Info, this.GetType().Name, $"DM sent to {serverUser.Username}");
            }
            catch (HttpException e)
            {
                await LogService.Log(LogSeverity.Error, this.GetType().Name, "User cannot receive DMs", e);
            }
            catch (Exception e)
            {
                await LogService.Log(LogSeverity.Error, this.GetType().Name, "Error with SendMessageAsync", e);
            }
        }

        distinctUserIds.ForEach(SendMessageToUser);
    }

    private async Task OnConnectionClosedAsync(Exception exception)
    {
        string errorMsg = exception != null ? exception.Message : "Connection error 'Merchants'";

        await LogService.Log(LogSeverity.Error, this.GetType().Name, errorMsg);
        await Task.Delay(2000);
        await this.StartConnectionAsync();
    }

    private async Task OnConnectionExceptionAsync(Exception exception) => await this.OnConnectionClosedAsync(exception);

    private void OnUpdateMerchantGroup() =>
        this.hubConnection.On<string, object>(
                                              "UpdateMerchantGroup",
                                              // ReSharper disable once UnusedParameter.Local
                                              async (server, merchantGroupObj) =>
                                              {
                                                  await this.UpdateMerchantGroupHandler(merchantGroupObj, false);
                                              });

    private void OnUpdateVotes() =>
        this.hubConnection.On<List<object>>(
                                            "UpdateVotes",
                                            async votes =>
                                            {
                                                List<WebsiteMerchant> activeMerchants = await JsonParsers.GetActiveMerchantsJsonAsync();
                                                List<MerchantVote> positiveVotes = new();
                                                List<MerchantVote> negativeVotes = new();

                                                foreach (MerchantVote vote in votes.Select(
                                                                                           obj => JsonSerializer.Deserialize<MerchantVote>(
                                                                                            obj.ToString() ?? string.Empty)))
                                                {
                                                    if (vote.Votes <= VoteThreshold)
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
                                                        await LogService.Log(
                                                                             LogSeverity.Debug,
                                                                             this.GetType().Name,
                                                                             "Could not find merchant message to modify. Triggering merchant update.");

                                                        object merchantGroupObj =
                                                            await this.hubConnection.InvokeAsync<object>("GetKnownActiveMerchantGroups", "Wei");

                                                        await this.UpdateMerchantGroupHandler(merchantGroupObj, true);

                                                        return;
                                                    }

                                                    merchantMessage.IsDeleted = true;
                                                    merchantMessage.MessageId = null;

                                                    IUserMessage message =
                                                        await this.textChannel.GetMessageAsync((ulong)merchantMessage.MessageId!) as IUserMessage;

                                                    await message.DeleteAsync();

                                                    await LogService.Log(
                                                                         LogSeverity.Info,
                                                                         this.GetType().Name,
                                                                         $"Deleted post for merchant with id {vote.Id}: {vote.Votes} votes");

                                                    WebsiteMerchant merchant = activeMerchants.Find(x => x.Id == vote.Id);
                                                    merchant.Votes = vote.Votes;
                                                }

                                                foreach (MerchantVote vote in positiveVotes)
                                                {
                                                    MerchantMessage merchantMessage = Program.MerchantMessages.Find(x => x.MerchantId == vote.Id);

                                                    if (merchantMessage == null)
                                                    {
                                                        await LogService.Log(
                                                                             LogSeverity.Debug,
                                                                             this.GetType().Name,
                                                                             "Could not find merchant message to modify. Triggering merchant update.");

                                                        object merchantGroupObj =
                                                            await this.hubConnection.InvokeAsync<object>("GetKnownActiveMerchantGroups", "Wei");

                                                        await this.UpdateMerchantGroupHandler(merchantGroupObj, true);

                                                        return;
                                                    }

                                                    IUserMessage message =
                                                        await this.textChannel.GetMessageAsync((ulong)merchantMessage.MessageId!) as IUserMessage;

                                                    IEmbed oldEmbed = message.Embeds.First();

                                                    EmbedBuilder newEmbed = oldEmbed.ToEmbedBuilder();
                                                    newEmbed.WithFooter($"Votes: {vote.Votes}");

                                                    await message.ModifyAsync(x => x.Embed = newEmbed.Build());

                                                    WebsiteMerchant merchant = activeMerchants.Find(x => x.Id == vote.Id);

                                                    if (merchant == null)
                                                    {
                                                        await LogService.Log(
                                                                             LogSeverity.Warning,
                                                                             this.GetType().Name,
                                                                             $"Could not find active merchant with id {vote.Id}, This should not happen normally");

                                                        continue;
                                                    }

                                                    merchant.Votes = vote.Votes;
                                                }

                                                await JsonParsers.WriteActiveMerchantsAsync(activeMerchants);
                                            });

    [SlashCommand("reconnect-merchant", "Re-connects to lostmerchants and starts posting merchant locations when available")]
    public async Task ReconnectMerchantChannel()
    {
        await this.DeferAsync(true);

        if (this.Context.User.Id != Config.Default.Admin)
        {
            await this.FollowupAsync("You don't have permission to use this command", ephemeral: true);

            return;
        }

        if (Config.Default.MerchantChannel == 0
         || this.textChannel == null)
        {
            await this.FollowupAsync("Merchant channel not set. Use /set-as-merchant-channel first", ephemeral: true);

            return;
        }

        if (this.Context.Channel.Id != Config.Default.MerchantChannel)
        {
            await this.FollowupAsync("This is not the merchant channel", ephemeral: true);

            return;
        }

        await this.StartMerchantChannel();
        await this.FollowupAsync("Success", ephemeral: true);
    }

    private async Task StartConnectionAsync()
    {
        try
        {
            if (this.hubConnection.State != HubConnectionState.Disconnected)
            {
                await this.hubConnection.StopAsync();
            }

            await this.hubConnection.StartAsync();
            this.hubConnection.Remove("SubscribeToServer");
            this.hubConnection.Remove("UpdateVotes");
            this.hubConnection.Remove("UpdateMerchantGroup");
            await this.hubConnection.InvokeAsync("SubscribeToServer", "Wei");
            this.OnUpdateMerchantGroup();
            this.OnUpdateVotes();
        }
        catch (Exception exception)
        {
            await this.OnConnectionExceptionAsync(exception);
        }
    }

    public async Task StartMerchantChannel()
    {
        this.textChannel = Program.MerchantChannel;
        string merchantInfoString = await new HttpClient().GetStringAsync("https://lostmerchants.com/data/merchants.json");
        this.merchantInfo = JsonSerializer.Deserialize<Dictionary<string, MerchantInfo>>(merchantInfoString);

        //hubConnection = new HubConnectionBuilder()
        //    .WithUrl("https://test.lostmerchants.com/MerchantHub")
        //    .ConfigureLogging(logging =>
        //    {
        //        logging.SetMinimumLevel(LogLevel.Debug);
        //        logging.AddProvider(new SignalLoggerProvider());
        //    })
        //    .Build();

        this.hubConnection = new HubConnectionBuilder()
                             .WithUrl("https://lostmerchants.com/MerchantHub")
                             .ConfigureLogging(
                                               logging =>
                                               {
                                                   logging.SetMinimumLevel(LogLevel.Debug);
                                                   logging.AddProvider(new SignalLoggerProvider());
                                               })
                             .Build();

        this.hubConnection.KeepAliveInterval = new TimeSpan(0, 4, 0);
        this.hubConnection.ServerTimeout = new TimeSpan(0, 8, 0);
        this.hubConnection.Closed -= this.OnConnectionClosedAsync;
        this.hubConnection.Closed += this.OnConnectionClosedAsync;

        List<IMessage> messages = await this.textChannel.GetMessagesAsync().Flatten().ToListAsync();
        await this.textChannel.DeleteMessagesAsync(messages);

        await this.StartConnectionAsync();

        DateTimeOffset now = DateTimeOffset.Now;

        //10:00 - 15:30 active
        //15:30 - 16:00 break
        //16:00 - 21:30 active
        //21:30 - 22:00 break
        //22:00 - 3:30 active
        //3:30 - 4:00 break
        //4:00 - 9:30 active
        //9:30 - 10:00 break

        //break
        if (now is
            {
                Minute: >= 30,
                Hour: 15 or 21 or 3 or 9,
            })
        {
            DateTimeOffset nextMerchantsTime = now.AddMinutes(-now.Minute).AddMinutes(30).AddSeconds(-now.Second);

            await this.textChannel.SendMessageAsync($"Next merchants: <t:{nextMerchantsTime.ToUnixTimeSeconds()}:R>");
        }
        else
        {
            object merchantGroupObj = await this.hubConnection.InvokeAsync<object>("GetKnownActiveMerchantGroups", "Wei");
            await this.UpdateMerchantGroupHandler(merchantGroupObj, true);
        }

        RestUserMessage msg = await this.textChannel.SendMessageAsync("**Merchant channel activated**\n*(This message will delete itself)*");
        Thread.Sleep(5 * 1000);
        await msg.DeleteAsync();
    }

    private async Task UpdateMerchantGroupHandler(object merchantGroupObj, bool triggeredManually)
    {
        List<WebsiteMerchant> jsonMerchants = await JsonParsers.GetActiveMerchantsJsonAsync();
        List<WebsiteMerchant> activeMerchants;

        if (!triggeredManually)
        {
            MerchantGroup merchantGroup = JsonSerializer.Deserialize<MerchantGroup>(merchantGroupObj.ToString()!);
            activeMerchants = merchantGroup.ActiveMerchants;

            if (jsonMerchants.Count == 0)
            {
                List<IMessage> messages = await this.textChannel.GetMessagesAsync().Flatten().ToListAsync();
                await this.textChannel.DeleteMessagesAsync(messages);
            }
        }
        else
        {
            List<MerchantGroup> merchantGroups = JsonSerializer.Deserialize<List<MerchantGroup>>(merchantGroupObj.ToString()!);
            activeMerchants = merchantGroups!.Select(x => x.ActiveMerchants.First()).ToList();

            Program.MerchantMessages = new List<MerchantMessage>();
            jsonMerchants = new List<WebsiteMerchant>();

            List<IMessage> messages = await this.textChannel.GetMessagesAsync().Flatten().ToListAsync();
            await this.textChannel.DeleteMessagesAsync(messages);
        }

        foreach (WebsiteMerchant merchant in activeMerchants)
        {
            WebsiteMerchant jsonStored = jsonMerchants.Find(x => x.Id == merchant.Id);

            if (jsonStored != null)
            {
                continue;
            }

            await LogService.Log(LogSeverity.Debug, this.GetType().Name, $"Adding merchant: {merchant.Name}");

            List<int> notableCard = new();
            List<int> notableRapport = new();
            string rolePing = "";

            if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Wei)))
            {
                rolePing += "<@&986032976812982343> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Wei);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Mokamoka)))
            {
                rolePing += "<@&986033361770385429> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Mokamoka);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Sian)))
            {
                rolePing += "<@&986033048271331428> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Sian);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Seria)))
            {
                rolePing += "<@&986033604205371463> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Seria);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Madnick)))
            {
                rolePing += "<@&986033108954525836> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Madnick);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Kaysarr)))
            {
                rolePing += "<@&986033435531419679> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Kaysarr);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Danika)))
            {
                rolePing += "<@&1078284489060515921> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Danika);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Arno)))
            {
                rolePing += "<@&1078284603787333742> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Arno);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Baskia)))
            {
                rolePing += "<@&1078284645763919952> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Baskia);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Anke)))
            {
                rolePing += "<@&1078284691767046214> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Anke);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Piela)))
            {
                rolePing += "<@&1078284729272504340> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Piela);
            }
            else if (merchant.Cards.Any(x => x.Name.Replace(" ", "") == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.RowenZenlord)))
            {
                rolePing += "<@&1078284769995006073> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.RowenZenlord);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Vairgrys)))
            {
                rolePing += "<@&1118825650703302758> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Vairgrys);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Thar)))
            {
                rolePing += "<@&1130243242168963142> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Thar);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Balthorr)))
            {
                rolePing += "<@&1130243853602013276> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Balthorr);
            }
            else if (merchant.Cards.Any(x => x.Name.Replace(" ", "") == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.DelainArmen)))
            {
                rolePing += "<@&1130244337943461898> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.DelainArmen);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Jederico)))
            {
                rolePing += "<@&1130244828995780699> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Jederico);
            }
            else if (merchant.Cards.Any(x => x.Name == Enum.GetName(typeof(WanderingMerchantItemsEnum), WanderingMerchantItemsEnum.Osphere)))
            {
                rolePing += "<@&1130245316541685791> ";
                notableCard.Add((int)WanderingMerchantItemsEnum.Osphere);
            }

            if (merchant.Rapports.Any(x => x.Rarity == Rarity.Legendary))
            {
                rolePing += "<@&986032866053996554>";
                notableRapport.Add((int)WanderingMerchantItemsEnum.LegendaryRapport);
            }

            if (string.IsNullOrEmpty(rolePing))
            {
                rolePing = "\u200b";
            }

            DateTimeOffset now = DateTimeOffset.Now;
            int hour = 0;

            switch (now.Hour)
            {
                case 22:
                case 23:
                case 0:
                case 1:
                case 2:
                case 3:
                    hour = 3;

                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    hour = 9;

                    break;
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    hour = 15;

                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                    hour = 21;

                    break;
            }

            DateTimeOffset expiryDate = new(now.Year, now.Month, now.Day, hour, 30, 0, now.Offset);

            Embed embed = this.CreateMerchantEmbed(merchant, expiryDate, MerchantEmbedTypeEnum.New).Build();

            jsonMerchants.Add(merchant);

            if (merchant.Votes <= VoteThreshold)
            {
                Program.MerchantMessages.Add(new MerchantMessage(merchant.Id, null, true));

                return;
            }

            IUserMessage message = await this.textChannel.SendMessageAsync(rolePing, embed: embed);
            Program.MerchantMessages.Add(new MerchantMessage(merchant.Id, message.Id));

            if (notableCard.Count != 0
             || notableRapport.Count != 0)
            {
                await this.GetUserSubscriptions(notableCard, notableRapport, merchant, expiryDate);
            }
        }

        if (triggeredManually && jsonMerchants.Count != activeMerchants.Count)
        {
            await LogService.Log(
                                 LogSeverity.Warning,
                                 this.GetType().Name,
                                 $"Number of merchants doesn't match. Merchant group: {activeMerchants.Count}, stored JSON: {jsonMerchants.Count}");
        }

        await JsonParsers.WriteActiveMerchantsAsync(jsonMerchants);
    }
}