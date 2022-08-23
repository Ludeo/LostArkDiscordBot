using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Bot;
using LostArkBot.Bot.FileObjects;
using LostArkBot.Bot.FileObjects.LostMerchants;
using LostArkBot.Bot.QuartzJobs;
using LostArkBot.Bot.Shared;
using LostArkBot.Bot.SlashCommands;
using LostArkBot.databasemodels;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace LostArkBot;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program
{
    private static DiscordSocketClient Client { get; set; }

    public static Random Random { get; } = new();

    public static SocketGuild Guild { get; private set; }

    public static List<GuildEmote> GuildEmotes { get; private set; }

    public static StaticObjects StaticObjects { get; private set; }

    public static SocketTextChannel MerchantChannel { get; set; }

    public static List<MerchantMessage> MerchantMessages { get; set; } = new();

    private static DiscordSocketConfig BuildDiscordSocketConfig()
    {
        DiscordSocketConfig config = new()
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildMembers,
            UseInteractionSnowflakeDate = false,
            AlwaysDownloadUsers = true,
        };

        return config;
    }

    private static ServiceProvider ConfigureServices() =>
        new ServiceCollection()
            .AddSingleton(new LostArkBotContext())
            .AddSingleton(new DiscordSocketClient(BuildDiscordSocketConfig()))
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandlingService>()
            .BuildServiceProvider();

    private static async Task InitializeEmotes()
    {
        IReadOnlyCollection<GuildEmote> emotes = await Guild.GetEmotesAsync();
        GuildEmotes = new List<GuildEmote>(emotes);
        Client.Ready -= InitializeEmotes;
    }

    private static Task InitializeGlobalVariables()
    {
        Guild = Client.GetGuild(Config.Default.Server);
        MerchantChannel = Guild.GetTextChannel(Config.Default.MerchantChannel);

        return Task.CompletedTask;
    }

    private static async Task InitializeScheduledTask()
    {
        StdSchedulerFactory stdSchedulerFactory = new();
        IScheduler scheduler = await stdSchedulerFactory.GetScheduler();

        await scheduler.Start();

        IJobDetail merchantJob = JobBuilder.Create<MerchantJob>()
                                           .WithIdentity("merchantjob", "merchantgroup")
                                           .Build();

        ITrigger merchantTrigger = TriggerBuilder.Create()
                                                 .WithIdentity("merchanttrigger", "merchantgroup")
                                                 .StartNow()
                                                 .WithCronSchedule("0 56 * * * ?")
                                                 .Build();

        await scheduler.ScheduleJob(merchantJob, merchantTrigger);
        Client.Ready -= InitializeScheduledTask;
    }

    private static void Main() => MainAsync().GetAwaiter().GetResult();

    private static async Task MainAsync()
    {
        ServiceProvider services = ConfigureServices();
        Client = services.GetRequiredService<DiscordSocketClient>();
        InteractionService commands = services.GetRequiredService<InteractionService>();
        CommandHandlingService commandHandlingService = services.GetRequiredService<CommandHandlingService>();

        await LogService.Log(LogSeverity.Info, "Setup", "============================================================================");
        await LogService.Log(LogSeverity.Info, "Setup", "=========================== Application starting ===========================");
        await LogService.Log(LogSeverity.Info, "Setup", "============================================================================");

        await commandHandlingService.Initialize();

        Client.Log += LogService.LogHandler;
        commands.Log += LogService.LogHandler;
        Client.Ready += InitializeGlobalVariables;
        Client.Ready += InitializeEmotes;
        Client.Ready += InitializeScheduledTask;
        Client.Ready += new MerchantModule(services.GetRequiredService<LostArkBotContext>()).StartMerchantChannel;

        StaticObjects = new StaticObjects(services.GetRequiredService<LostArkBotContext>());

        string token = Config.Default.Token;

        if (string.IsNullOrEmpty(token))
        {
            await LogService.Log(
                                 LogSeverity.Critical,
                                 "Setup",
                                 "The bot token is not available in the config.json file. Add it and restart the bot.");

            Environment.Exit(0);
        }

        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
        await Client.SetGameAsync("Lost Ark || /help");

        await Task.Delay(Timeout.Infinite);
    }

    public static void ReinitializeScheduledTasks()
    {
        Client.Ready -= InitializeScheduledTask;
        Client.Ready += InitializeScheduledTask;
    }
}