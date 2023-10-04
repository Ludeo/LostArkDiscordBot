using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Bot.Handlers;
using LostArkBot.Bot.Shared;
using LostArkBot.databasemodels;
using Microsoft.Extensions.DependencyInjection;

namespace LostArkBot.Bot;

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

    private static IInteractionContext CreateGeneric(DiscordSocketClient client, SocketInteraction interaction) =>
        interaction switch
        {
            SocketUserCommand user           => new SocketInteractionContext<SocketUserCommand>(client, user),
            SocketSlashCommand slash         => new SocketInteractionContext<SocketSlashCommand>(client, slash),
            SocketMessageCommand command     => new SocketInteractionContext<SocketMessageCommand>(client, command),
            SocketMessageComponent component => new SocketInteractionContext<SocketMessageComponent>(client, component),
            SocketModal modal                => new SocketInteractionContext<SocketModal>(client, modal),
            _                                => throw new InvalidOperationException("This interaction type is not supported!"),
        };

    public async Task Initialize()
    {
        try
        {
            await this.commands.AddModulesAsync(Assembly.GetExecutingAssembly(), this.services);
            this.client.InteractionCreated += this.InteractionCreated;
            this.client.SelectMenuExecuted += new MenuHandlerClass(this.services.GetRequiredService<LostArkBotContext>()).MenuHandler;
            this.client.ModalSubmitted += new ModalHandlers(this.services.GetRequiredService<LostArkBotContext>()).ModalHandler;
            this.client.Ready += this.Ready;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            throw;
        }
    }

    private async Task InteractionCreated(SocketInteraction arg)
    {
        IInteractionContext ctx = CreateGeneric(this.client, arg);
        await this.commands.ExecuteCommandAsync(ctx, this.services);
    }

    private async Task Ready()
    {
        await this.RegisterCommands();

        foreach (SocketGuild guild in this.client.Guilds)
        {
            await LogService.Log(LogSeverity.Info, "Ready", "Connected to " + guild.Name);
        }

        this.client.Ready -= this.Ready;
    }

    private async Task RegisterCommands()
    {
        await this.client.BulkOverwriteGlobalApplicationCommandsAsync(new List<ApplicationCommandProperties>().ToArray());

        /*await this.client.GetGuild(Config.Default.Server)
                  .BulkOverwriteApplicationCommandAsync(new List<ApplicationCommandProperties>().ToArray());

        await this.commands.RegisterCommandsToGuildAsync(Config.Default.Server);*/
        await this.commands.RegisterCommandsGloballyAsync();
    }
}