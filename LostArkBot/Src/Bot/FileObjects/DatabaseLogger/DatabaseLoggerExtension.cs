using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace LostArkBot.Bot.FileObjects.DatabaseLogger;

public static class DatabaseLoggerExtension
{
    public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder)
    {
        builder.AddConfiguration();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DatabaseLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<DatabaseLoggerConfiguration, DatabaseLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddDatabaseLogger(this ILoggingBuilder builder, Action<DatabaseLoggerConfiguration> configure)
    {
        builder.AddDatabaseLogger();
        builder.Services.Configure(configure);

        return builder;
    }
}