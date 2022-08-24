using System;
using System.Threading.Tasks;
using Discord;
using LostArkBot.Bot.Shared;
using Microsoft.Extensions.Logging;

namespace LostArkBot.Bot.FileObjects.DatabaseLogger;

public class DatabaseLogger : ILogger
{
    // ReSharper disable once NotAccessedField.Local
    private readonly string name;
    private readonly Func<DatabaseLoggerConfiguration> getCurrentConfig;

    public DatabaseLogger(string name, Func<DatabaseLoggerConfiguration> getCurrentConfig) => (this.name, this.getCurrentConfig) = (name, getCurrentConfig);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        LogSeverity discordLogLevel = LogSeverity.Info;

        switch (logLevel)
        {
            case LogLevel.Critical: discordLogLevel = LogSeverity.Critical;

                break;
            case LogLevel.Debug: discordLogLevel = LogSeverity.Debug;

                break;
            case LogLevel.Error: discordLogLevel = LogSeverity.Error;

                break;
            case LogLevel.None: return;
            case LogLevel.Trace: discordLogLevel = LogSeverity.Verbose;

                break;
            case LogLevel.Warning: discordLogLevel = LogSeverity.Warning;

                break;
            case LogLevel.Information: break;
            default:                   throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }

        if (exception != null)
        {
            Task<Task> task = LogService.Log(discordLogLevel, this.GetType().Name, exception.Message, exception);
            task.GetAwaiter().GetResult();
        }
    }

    public bool IsEnabled(LogLevel logLevel) => this.getCurrentConfig().LogLevels.ContainsKey(logLevel);

    public IDisposable BeginScope<TState>(TState state) => default!;
}