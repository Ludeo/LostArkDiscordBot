using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LostArkBot.Bot;
using LostArkBot.Bot.FileObjects;
using LostArkBot.Bot.Modules;
using LostArkBot.Src.Bot;
using Microsoft.Extensions.DependencyInjection;
using Timer = System.Timers.Timer;

namespace LostArkBot
{
    /// <summary>
    ///     Class that gets executed when the program starts, it also starts up the bot.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Gets the timer that is needed for the score tracking.
        /// </summary>
        public static Timer Timer { get; } = new();

        /// <summary>
        ///     Gets the discord client.
        /// </summary>
        public static DiscordSocketClient Client { get; private set; }

        public static Random random { get; } = new Random();

        private static Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            File.AppendAllText("log.txt", log + "\n");

            return Task.CompletedTask;
        }

        private static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            ServiceProvider services = this.ConfigureServices();

            Client = services!.GetRequiredService<DiscordSocketClient>();

            Client.Log += Log;
            services!.GetRequiredService<CommandService>().Log += Log;

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

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            string prefix = config.Prefix;

            await Client.SetGameAsync($"Lost Ark || /help");

            Client.Ready += this.ClientReady;
            StartMerchantModule.StartMerchantAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private ServiceProvider ConfigureServices() => new ServiceCollection()
                                                       .AddSingleton<DiscordSocketClient>()
                                                       .AddSingleton<CommandService>()
                                                       .AddSingleton<CommandHandlingService>()
                                                       .BuildServiceProvider();

        private async Task ClientReady()
        {
            await SlashCommandInitialization.CreateSlashCommands();
        }
    }
}