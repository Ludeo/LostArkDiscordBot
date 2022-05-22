using Discord;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using LostArkBot.Src.Bot.SlashCommands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot
{
    internal class SlashCommandInitialization
    {
        public static async Task CreateSlashCommands()
        {
            List<ApplicationCommandProperties> applicationCommandProperties = new();

            try
            {
                SocketGuild guild = Program.Client.GetGuild(Config.Default.Server);

                //test server
                //SocketGuild guild = Program.Client.GetGuild(340543173422088212);

                applicationCommandProperties.Add(StartMerchantInitialization.StartMerchant().Build());

                applicationCommandProperties.Add(RegisterInitialization.Register().Build());

                applicationCommandProperties.Add(UpdateInitialization.Update().Build());

                applicationCommandProperties.Add(ProfileInitialization.Profile().Build());

                applicationCommandProperties.Add(LfgInitialization.Lfg().Build());

                applicationCommandProperties.Add(AccountInitialization.Account().Build());

                applicationCommandProperties.Add(DeleteInitialization.Delete().Build());

                applicationCommandProperties.Add(EditMessageInitialization.EditMessage().Build());

                applicationCommandProperties.Add(ServerStatusInitialization.ServerStatus().Build());

                applicationCommandProperties.Add(HelpInitialization.Help().Build());

                applicationCommandProperties.Add(RollInitialization.Roll().Build());

                await guild.BulkOverwriteApplicationCommandAsync(applicationCommandProperties.ToArray());
            }
            catch (HttpException exception)
            {
                string log = JsonSerializer.Serialize(exception.Errors);
                Console.WriteLine(log);
                await File.AppendAllTextAsync("log.txt", log);
            }
        }
    }
}