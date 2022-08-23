using System;
using System.Threading.Tasks;
using Discord;
using LostArkBot.Bot.Shared;
using Microsoft.Extensions.Logging;

namespace LostArkBot.Bot.FileObjects.SignalR;

public class SignalLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) => this.BeginScopeAsync(state).GetAwaiter().GetResult();

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        LogSeverity discordLogLevel = LogSeverity.Info;

        switch (logLevel)
        {
            case LogLevel.Critical: discordLogLevel = LogSeverity.Critical;

                break;
            case LogLevel.Debug:    discordLogLevel = LogSeverity.Debug;

                break;
            case LogLevel.Error:    discordLogLevel = LogSeverity.Error;

                break;
            case LogLevel.None:     return;
            case LogLevel.Trace:    discordLogLevel = LogSeverity.Verbose;

                break;
            case LogLevel.Warning:  discordLogLevel = LogSeverity.Warning;

                break;
            case LogLevel.Information: break;
            default:                   throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }

        if (exception != null)
        {
            await LogService.Log(discordLogLevel, this.GetType().Name, exception.Message, exception);
        }
    }

    private async Task<IDisposable> BeginScopeAsync<TState>(TState state)
    {
        await LogService.Log(LogSeverity.Info, this.GetType().Name, "Begging Scope with state: " + state);

        return default!;
    }
}