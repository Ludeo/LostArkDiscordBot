using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Bot;
using LostArkBot.Src.Bot.FileObjects;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using System.Collections.Generic;
using Discord.Commands;

namespace LostArkBot
{
    public class Program
    {
        private static DiscordSocketClient Client { get; set; }

        public static Random Random { get; } = new Random();

        public static List<GuildEmote> GuildEmotes { get; private set; }

        public static Task Log(LogMessage log)
        {
            string text = $"[General/{log.Severity}] {log}";
            
            if(log.Exception is CommandException commandException)
            {
                text = $"[Command/{log.Severity}] {commandException.Command.Name} failed to execute in {commandException.Context.Channel.Name}\n{commandException}";
            }

            if(log.Severity == LogSeverity.Critical)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            } else if(log.Severity == LogSeverity.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            } else if (log.Severity == LogSeverity.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            } else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine(text);
            File.AppendAllText("log.txt", text + "\n");

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
            Client.Ready += InitializeEmotes;

            Config config = Config.Default;

            if (string.IsNullOrEmpty(config.Token))
            {
                LogMessage logMessage = new(LogSeverity.Critical, "Setup", "The bot token is not available in the config.json file. Add it and restart the bot.");
                await Log(logMessage);
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

        private static DiscordSocketConfig BuildDiscordSocketConfig()
        {
            DiscordSocketConfig config = new()
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages,
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