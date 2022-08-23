using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace LostArkBot.Bot.Shared;

public static class LogService
{
    public static async Task<Task> Log(LogSeverity severity, string source, string message, Exception exception = null)
    {
        LogMessage log = new(severity, source, message, exception);
        string text = $"[General/{log.Severity}] {log.ToString(padSource: 15)}";
        string logFileName = $"Logs/log_{DateTime.Now:MM_dd_yyyy}.txt";

        if (log.Exception is CommandException commandException)
        {
            text =
                $"[Command/{log.Severity}] {commandException.Command.Name} failed to execute in {commandException.Context.Channel.Name}\n{commandException}";
        }

        Console.ForegroundColor = log.Severity switch
        {
            LogSeverity.Critical => ConsoleColor.DarkRed,
            LogSeverity.Error    => ConsoleColor.Red,
            LogSeverity.Warning  => ConsoleColor.Yellow,
            LogSeverity.Debug    => ConsoleColor.Cyan,
            LogSeverity.Info     => ConsoleColor.Green,
            LogSeverity.Verbose  => ConsoleColor.Magenta,
            _                    => ConsoleColor.White,
        };

        Console.WriteLine(text);

        await TryWritingToLog(text, logFileName);

        return Task.CompletedTask;
    }

    public static async Task<Task> LogHandler(LogMessage log) => await Log(log.Severity, log.Source, log.Message, log.Exception);

    private static async Task TryWritingToLog(string text, string logFileName)
    {
        try
        {
            await File.AppendAllTextAsync(logFileName, text + "\n");
        }
        catch (DirectoryNotFoundException)
        {
            Directory.CreateDirectory("Logs");
        }
        catch
        {
            LogMessage log = new(LogSeverity.Debug, "LogService", "Error in writing to log, retrying...\n");
            await File.AppendAllTextAsync(logFileName, $"[General/{log.Severity}] {log.ToString(padSource: 15)}");
            await Task.Delay(500);
            await TryWritingToLog(text, logFileName);
        }
    }
}