using Discord;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Shared
{
    public class LogService
    {

        public static async Task<Task> LogHandler(LogMessage log)
        {
            return await Log(log.Severity, log.Source, log.Message, log.Exception);
        }

        public static async Task<Task> Log(LogSeverity severity, string source, string message, Exception exception = null)
        {
            LogMessage log = new(severity, source, message, exception);
            string text = $"[General/{log.Severity}] {log.ToString(padSource: 15)}";
            string logFileName = $"Logs/log_{DateTime.Now:MM_dd_yyyy}.txt";

            if (log.Exception is CommandException commandException)
            {
                text = $"[Command/{log.Severity}] {commandException.Command.Name} failed to execute in {commandException.Context.Channel.Name}\n{commandException}";
            }

            if (log.Severity == LogSeverity.Critical)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }
            else if (log.Severity == LogSeverity.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (log.Severity == LogSeverity.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (log.Severity == LogSeverity.Debug)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (log.Severity == LogSeverity.Info)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (log.Severity == LogSeverity.Verbose)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine(text);

            await TryWritingToLog(text, logFileName);

            return Task.CompletedTask;
        }

        private static async Task TryWritingToLog(string text, string logFileName)
        {
            try
            {
                File.AppendAllText(logFileName, text + "\n");
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory("Logs");
            }
            catch
            {
                LogMessage log = new(LogSeverity.Debug, "LogService", $"Error in writing to log, retrying...\n", null);
                File.AppendAllText(logFileName, $"[General/{log.Severity}] {log.ToString(padSource: 15)}");
                await Task.Delay(500);
                await TryWritingToLog(text, logFileName);
            }
        }
    }
}