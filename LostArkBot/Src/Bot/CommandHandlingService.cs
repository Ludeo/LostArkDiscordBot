using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Bot.Modules;
using LostArkBot.Src.Bot.Handlers;
using Microsoft.Extensions.DependencyInjection;
using IResult = Discord.Commands.IResult;

namespace LostArkBot.Bot
{
    /// <summary>
    ///     Service that handles the commands for discord.
    /// </summary>
    public class CommandHandlingService
    {
        private static DiscordSocketClient client;
        private static CommandService commands;
        private static IServiceProvider services;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandHandlingService"/> class.
        /// </summary>
        /// <param name="services"> The service provider of the services. </param>
        public CommandHandlingService(IServiceProvider services)
        {
            CommandHandlingService.services = services;

            client = services!.GetRequiredService<DiscordSocketClient>();
            commands = services.GetRequiredService<CommandService>();

            commands.CommandExecuted += CommandExecutedAsync;
            client.MessageReceived += this.MessagedReceivedAsync;
            client.SlashCommandExecuted += SlashCommandHandlerClass.SlashCommandHandler;
            client.ButtonExecuted += ButtonHandlerClass.ButtonHandler;
            client.SelectMenuExecuted += MenuHandlerClass.MenuHandler;
        }

        /// <summary>
        ///     Initializes the CommandHandlingService.
        /// </summary>
        /// <returns> An empty task. </returns>
        public async Task InitializeAsync() => await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

        private async Task MessagedReceivedAsync(SocketMessage rawMessage)
        {
            int argPos = 0;

            SocketGuildChannel channel = rawMessage.Channel as SocketGuildChannel;

            if (rawMessage.Channel.Id == Config.Default.MerchantChannel && channel.Guild.Id == Config.Default.Server)
            {
                PingMerchantRolesModule rolesModule = new();
                await rolesModule.StartPingMerchantRolesAsync(rawMessage);
            }

            if (rawMessage is SocketUserMessage { Source: MessageSource.User } message && message.HasStringPrefix(Config.Default.Prefix, ref argPos))
            {
                SocketCommandContext context = new(client, message);

                await commands.ExecuteAsync(context, argPos, services);
            }
        }

        private static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            string message = context.Message.ToString();

            if (!command.IsSpecified)
            {
                LogMessage notSpecifiedLog = new(LogSeverity.Error, "Command", message);
                Console.WriteLine(notSpecifiedLog + " || Command doesn't exist");
                await File.AppendAllTextAsync("log.txt", notSpecifiedLog + " || Command doesn't exist\n");
            }
            else if (result.IsSuccess)
            {
                LogMessage successLog = new(LogSeverity.Info, "Command", message);
                Console.WriteLine(successLog);
                await File.AppendAllTextAsync("log.txt", successLog + "\n");
            }
            else
            {
                await context.Channel.SendMessageAsync($"error: {result}");

                LogMessage errorLog = new(LogSeverity.Error, "Command", message);
                Console.WriteLine(errorLog + " || " + result);
                await File.AppendAllTextAsync("log.txt", errorLog + " || " + result + "\n");
            }
        }
    }
}