using Discord;
using LostArkBot.Src.Bot.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.FileObjects.SignalR
{
    public class SignalLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return BeginScopeAsync(state).GetAwaiter().GetResult();
        }

        public async Task<IDisposable> BeginScopeAsync<TState>(TState state)
        {
            await LogService.Log(LogSeverity.Info, this.GetType().Name, "Begging Scope with state: " + state);
            return default!;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.None)
            {
                return false;
            }

            return true;
        }

        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogSeverity discordLogLevel = LogSeverity.Info;

            if (logLevel == LogLevel.Critical)
            {
                discordLogLevel = LogSeverity.Critical;
            }
            else if (logLevel == LogLevel.Debug)
            {
                discordLogLevel = LogSeverity.Debug;
            }
            else if (logLevel == LogLevel.Error)
            {
                discordLogLevel = LogSeverity.Error;
            }
            else if (logLevel == LogLevel.None)
            {
                return;
            }
            else if (logLevel == LogLevel.Trace)
            {
                discordLogLevel = LogSeverity.Verbose;
            }
            else if (logLevel == LogLevel.Warning)
            {
                discordLogLevel = LogSeverity.Warning;
            }

            if (exception != null)
            {
                await LogService.Log(discordLogLevel, this.GetType().Name, exception.Message, exception);
            }
            else
            {
                await LogService.Log(discordLogLevel, this.GetType().Name, $"EventId: {eventId}, TState: {state}");
            }
        }
    }
}