using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Bot;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Bot.Modules;
using LostArkBot.Src.Bot;
using Microsoft.Extensions.DependencyInjection;
using Timer = System.Timers.Timer;
using System.Collections.Generic;
using Discord.Interactions;

namespace LostArkBot
{
    public class Program
    {
        public static Timer Timer { get; } = new();

        public static DiscordSocketClient Client { get; private set; }

        public static Random Random { get; } = new Random();

        public static Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            File.AppendAllText("log.txt", log + "\n");

            return Task.CompletedTask;
        }

        private static void Main() => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            ServiceProvider services = ConfigureServices();

            Client = services.GetRequiredService<DiscordSocketClient>();
            InteractionService commands = services.GetRequiredService<InteractionService>();
            CommandHandlingService commandHandlingService = services.GetRequiredService<CommandHandlingService>();

            await commandHandlingService.Initialize();

            Client.Log += Log;
            commands.Log += Log;

            Config config = Config.Default;

            if (string.IsNullOrEmpty(config.Token))
            {
                string log = "The bot token is not available in the config.json file. Add it and restart the bot.";
                Console.WriteLine(log);
                await File.AppendAllTextAsync("log.txt", log);
                Environment.Exit(0);
            }

            string token = config.Token;

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();
            await Client.SetGameAsync($"Lost Ark || /help");

            StartMerchantModule.StartMerchantAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private static ServiceProvider ConfigureServices() => new ServiceCollection()
                                                       .AddSingleton<DiscordSocketClient>()
                                                       .AddSingleton<InteractionService>()
                                                       .AddSingleton<CommandHandlingService>()
                                                       .BuildServiceProvider();

        private async Task ClientReady()
        {
            List<ApplicationCommandProperties> applicationCommandProperties = new();
            applicationCommandProperties.AddRange(await SlashCommandInitialization.CreateSlashCommands());
            applicationCommandProperties.AddRange(await GuildUserCommandInitialization.CreateGuildUserCommands());

            await Client.GetGuild(Config.Default.Server).BulkOverwriteApplicationCommandAsync(applicationCommandProperties.ToArray());
        }
    }
}