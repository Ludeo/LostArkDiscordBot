using Discord;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot
{
    internal class GuildUserCommandInitialization
    {
        public static async Task<List<ApplicationCommandProperties>> CreateGuildUserCommands()
        {
            List<ApplicationCommandProperties> applicationCommandProperties = new();

            try
            {
                applicationCommandProperties.Add(new UserCommandBuilder().WithName("characters").Build());

                return applicationCommandProperties;
            }
            catch (HttpException exception)
            {
                string log = JsonSerializer.Serialize(exception.Errors);
                Console.WriteLine(log);
                await File.AppendAllTextAsync("log.txt", log);

                return null;
            }
        }
    }
}
