﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Bot;
using LostArkBot.Src.Bot.FileObjects;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using System.Collections.Generic;
using Quartz.Impl;
using Quartz;
using LostArkBot.Src.Bot.QuartzJobs;
using LostArkBot.Src.Bot.Utils;

namespace LostArkBot
{
    public class Program
    {
        private static DiscordSocketClient Client { get; set; }

        public static Random Random { get; } = new Random();

        public static List<GuildEmote> GuildEmotes { get; private set; }

        public static StaticObjects StaticObjects { get; } = new StaticObjects();

        public static SocketTextChannel MerchantChannel { get; private set; }

        private static void Main() => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            ServiceProvider services = ConfigureServices();
            Client = services.GetRequiredService<DiscordSocketClient>();
            InteractionService commands = services.GetRequiredService<InteractionService>();
            CommandHandlingService commandHandlingService = services.GetRequiredService<CommandHandlingService>();

            LogMessage logMessage = new(LogSeverity.Info, "Setup", "============================================================================");
            await LogService.Log(logMessage);
            logMessage = new(LogSeverity.Info, "Setup", "=========================== Application starting ===========================");
            await LogService.Log(logMessage);
            logMessage = new(LogSeverity.Info, "Setup", "============================================================================");
            await LogService.Log(logMessage);

            await commandHandlingService.Initialize();

            Client.Log += LogService.Log;
            commands.Log += LogService.Log;
            Client.Ready += InitializeEmotes;
            Client.Ready += InitializeScheduledTask;

            Config config = Config.Default;

            if (string.IsNullOrEmpty(config.Token))
            {
                logMessage = new(LogSeverity.Critical, "Setup", "The bot token is not available in the config.json file. Add it and restart the bot.");
                await LogService.Log(logMessage);
                Environment.Exit(0);
            }

            string token = config.Token;

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();
            await Client.SetGameAsync($"Lost Ark || /help");

            await Task.Delay(Timeout.Infinite);
        }

        private static async Task InitializeEmotes()
        {
            IReadOnlyCollection<GuildEmote> emotes = await Client.GetGuild(Config.Default.Server).GetEmotesAsync();
            GuildEmotes = new(emotes);
            Client.Ready -= InitializeEmotes;
        }

        private static async Task InitializeScheduledTask()
        {
            MerchantChannel = Client.GetGuild(Config.Default.Server).GetTextChannel(Config.Default.MerchantChannel);
            StdSchedulerFactory stdSchedulerFactory = new();
            IScheduler scheduler = await stdSchedulerFactory.GetScheduler();

            await scheduler.Start();

            IJobDetail merchantJob = JobBuilder.Create<MerchantJob>()
                .WithIdentity("merchantjob", "merchantgroup")
                .Build();

            ITrigger merchantTrigger = TriggerBuilder.Create()
                .WithIdentity("merchanttrigger", "merchantgroup")
                .StartNow()
                .WithCronSchedule("* 56 * * * ?")
                .Build();

            await scheduler.ScheduleJob(merchantJob, merchantTrigger);
            Client.Ready -= InitializeScheduledTask;
        }

        private static DiscordSocketConfig BuildDiscordSocketConfig()
        {
            DiscordSocketConfig config = new()
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages,
                UseInteractionSnowflakeDate = false,
            };

            return config;
        }

        private static ServiceProvider ConfigureServices() => new ServiceCollection()
                                                       .AddSingleton(new DiscordSocketClient(BuildDiscordSocketConfig()))
                                                       .AddSingleton<InteractionService>()
                                                       .AddSingleton<CommandHandlingService>()
                                                       .BuildServiceProvider();
    }
}