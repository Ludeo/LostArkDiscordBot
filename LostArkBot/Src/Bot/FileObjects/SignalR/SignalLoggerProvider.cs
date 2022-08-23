using System;
using System.Threading.Tasks;
using Discord;
using LostArkBot.Bot.Shared;
using Microsoft.Extensions.Logging;

namespace LostArkBot.Bot.FileObjects.SignalR;

public class SignalLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new SignalLogger();

    public void Dispose()
    {
        this.DisposeAsync().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }

    private async Task DisposeAsync() => await LogService.Log(LogSeverity.Info, this.GetType().Name, "Disposing SignalLoggerProvider");
}