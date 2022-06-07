using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using Discord.Interactions;
using LostArkBot.Src.Bot.Handlers;

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
                client.SelectMenuExecuted += MenuHandlerClass.MenuHandler;
                client.Ready += Ready;
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

            foreach(SocketGuild guild in client.Guilds)
            {
                LogMessage connectedGuild = new(LogSeverity.Info, "Ready", "Connected to " + guild.Name);
                await Program.Log(connectedGuild);
            }

            client.Ready -= Ready;
        }

        private async Task RegisterCommands()
        {
            try
            {
#if DEBUG
                await commands.RegisterCommandsToGuildAsync(Config.Default.Server);
#else
                await commands.RegisterCommandsGloballyAsync();
#endif
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ButtonExecuted(SocketMessageComponent arg)
        {
            try
            {
                var ctx = new SocketInteractionContext<SocketMessageComponent>(client, arg);
                await commands.ExecuteCommandAsync(ctx, services);
            } catch(Exception)
            {
                throw;
            }
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
            return Task.CompletedTask;
        }
    }
}