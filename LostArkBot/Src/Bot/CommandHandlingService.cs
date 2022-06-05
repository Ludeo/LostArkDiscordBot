using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Bot.Modules;
using Discord.Interactions;

namespace LostArkBot.Bot
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient client;
        private readonly InteractionService commands;
        private readonly IServiceProvider services;

        public CommandHandlingService(InteractionService commands, DiscordSocketClient client, IServiceProvider services)
        {
            this.services = services;
            this.client = client;
            this.commands = commands;
        }

        public async Task Initialize() 
        {
            try
            {
                await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);
                client.InteractionCreated += InteractionCreated;
                client.ButtonExecuted += ButtonExecuted;
                client.Ready += Ready;
                client.MessageReceived += MessagedReceivedAsync;
                commands.SlashCommandExecuted += SlashCommandExecuted;
                commands.AutocompleteHandlerExecuted += AutoCompleteHandlerExecuted;
            } catch (Exception)
            {
                throw;
            }
        }

        private Task AutoCompleteHandlerExecuted(IAutocompleteHandler arg1, IInteractionContext arg2, IResult arg3)
        {
            return Task.CompletedTask;
        }

        private async Task Ready()
        {
            await RegisterCommands();
            client.Ready -= Ready;
        }

        private async Task RegisterCommands()
        {
            try
            {
                Console.WriteLine("registering commands");
                await commands.RegisterCommandsToGuildAsync(Config.Default.Server);
            } catch (Exception)
            {
                throw;
            }
        }

        private async Task ButtonExecuted(SocketMessageComponent arg)
        {
            var ctx = new SocketInteractionContext<SocketMessageComponent>(client, arg);
            await commands.ExecuteCommandAsync(ctx, services);
        }

        private async Task InteractionCreated(SocketInteraction arg)
        {
            try
            {
                var ctx = new SocketInteractionContext(client, arg);
                var result = await commands.ExecuteCommandAsync(ctx, services);
            } catch(Exception)
            {
                throw;
            }
        }

        private Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            Console.WriteLine("command got executed");
            return Task.CompletedTask;
        }

        private async Task MessagedReceivedAsync(SocketMessage rawMessage)
        {
            SocketGuildChannel channel = rawMessage.Channel as SocketGuildChannel;

            if (rawMessage.Channel.Id == Config.Default.MerchantChannel && channel.Guild.Id == Config.Default.Server)
            {
                PingMerchantRolesModule rolesModule = new();
                await rolesModule.StartPingMerchantRolesAsync(rawMessage);
            }
        }

        /*private static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
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
        }*/
    }
}