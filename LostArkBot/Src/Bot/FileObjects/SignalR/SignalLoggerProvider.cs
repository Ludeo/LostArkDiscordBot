using Discord;
using LostArkBot.Src.Bot.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.FileObjects.SignalR
{
    public class SignalLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new SignalLogger();
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        public async Task DisposeAsync()
        {
            await LogService.Log(LogSeverity.Info, this.GetType().Name, "Disposing SignalLoggerProvider");
            GC.SuppressFinalize(this);
        }
    }
}