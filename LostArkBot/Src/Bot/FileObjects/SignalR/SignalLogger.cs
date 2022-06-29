using Discord;
using Microsoft.Extensions.Logging;
using System;

namespace LostArkBot.Src.Bot.FileObjects.SignalR
{
    public class SignalLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            Program.Log(new LogMessage(LogSeverity.Info, "SignalLogging", "Begging Scope with state: " + state));
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

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
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
                Program.Log(new LogMessage(discordLogLevel, "SignalLogging", exception.Message, exception));
            }
            else
            {
                Program.Log(new LogMessage(discordLogLevel, "SignalLogging", $"EventId: {eventId}, TState: {state}"));
            }
        }
    }
}