using Discord;
using LostArkBot.Src.Bot.Utils;
using Microsoft.Extensions.Logging;
using System;

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
            LogService.Log(new LogMessage(LogSeverity.Info, "SignalLogging", "Disposing SignalLoggerProvider"));
            GC.SuppressFinalize(this);
        }
    }
}