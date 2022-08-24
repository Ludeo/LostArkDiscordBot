using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LostArkBot.Bot.FileObjects.DatabaseLogger;

public class DatabaseLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, DatabaseLogger> loggers = new(StringComparer.OrdinalIgnoreCase);
    private DatabaseLoggerConfiguration currentConfig;
    private readonly IDisposable onChangeToken;

    public DatabaseLoggerProvider(IOptionsMonitor<DatabaseLoggerConfiguration> config)
    {
        this.currentConfig = config.CurrentValue;
        this.onChangeToken = config.OnChange(updatedConfig => this.currentConfig = updatedConfig);
    }

    public ILogger CreateLogger(string categoryName) => this.loggers.GetOrAdd(categoryName, name => new DatabaseLogger(name, this.GetCurrentConfig));

    private DatabaseLoggerConfiguration GetCurrentConfig() => this.currentConfig;

    public void Dispose()
    {
        this.loggers.Clear();
        this.onChangeToken.Dispose();
        GC.SuppressFinalize(this);
    }
}