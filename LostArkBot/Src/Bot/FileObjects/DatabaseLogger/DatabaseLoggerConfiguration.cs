using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace LostArkBot.Bot.FileObjects.DatabaseLogger;

public class DatabaseLoggerConfiguration
{
    public int EventId { get; set; }

    public Dictionary<LogLevel, ConsoleColor> LogLevels { get; set; } = new()
    {
        [LogLevel.Information] = ConsoleColor.Green,
    };
}