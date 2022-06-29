﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using Discord.Interactions;
using LostArkBot.Src.Bot.Handlers;
using LostArkBot.Src.Bot.Utils;

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
                client.SelectMenuExecuted += MenuHandlerClass.MenuHandler;
                client.Ready += Ready;
            } catch (Exception)
            {
                throw;
            }
        }

        private async Task Ready()
        {
            await RegisterCommands();

            foreach(SocketGuild guild in client.Guilds)
            {
                LogMessage connectedGuild = new(LogSeverity.Info, "Ready", "Connected to " + guild.Name);
                await LogService.Log(connectedGuild);
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

        private async Task InteractionCreated(SocketInteraction arg)
        {
            var ctx = CreateGeneric(client, arg);
            await commands.ExecuteCommandAsync(ctx, services);
        }

        private static IInteractionContext CreateGeneric(DiscordSocketClient client, SocketInteraction interaction) => interaction switch
        {
            SocketUserCommand user => new SocketInteractionContext<SocketUserCommand>(client, user),
            SocketSlashCommand slash => new SocketInteractionContext<SocketSlashCommand>(client, slash),
            SocketMessageCommand command => new SocketInteractionContext<SocketMessageCommand>(client, command),
            SocketMessageComponent component => new SocketInteractionContext<SocketMessageComponent>(client, component),
            SocketModal modal => new SocketInteractionContext<SocketModal>(client, modal),
            _ => throw new InvalidOperationException("This interaction type is not supported!")
        };
    }
}