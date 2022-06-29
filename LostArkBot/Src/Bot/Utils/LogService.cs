﻿using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Utils
{
    internal class LogService
    {
        public static async Task<Task> Log(LogMessage log)
        {
            string text = $"[General/{log.Severity}] {log}";
            string logFileName = $"Logs\\log_{DateTime.Now.ToString("MM_dd_yyyy")}.txt";

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
                File.AppendAllText(logFileName, $"Error in writing to log, retrying..." + "\n");
                await Task.Delay(500);
                await TryWritingToLog(text, logFileName);
            }
        }
    }
}
