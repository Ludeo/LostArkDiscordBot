using Discord;
using LostArkBot.Src.Bot.Utils;
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
            await LogService.Log(new LogMessage(LogSeverity.Info, "SignalLogging", "Disposing SignalLoggerProvider"));
            GC.SuppressFinalize(this);
        }
    }
}