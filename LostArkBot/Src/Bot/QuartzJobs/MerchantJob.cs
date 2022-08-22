using Discord;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using LostArkBot.Src.Bot.FileObjects.LostMerchants;
using LostArkBot.Src.Bot.Shared;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.QuartzJobs
{
    public class MerchantJob : IJob
    {
        private readonly LostArkBotContext dbcontext;

        public MerchantJob(LostArkBotContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await LogService.Log(LogSeverity.Info, GetType().Name, "Executing Quartz job");
            SocketTextChannel textChannel = Program.MerchantChannel;
            List<IMessage> messages = await textChannel.GetMessagesAsync().Flatten().ToListAsync();

            await textChannel.DeleteMessagesAsync(messages);
            DateTimeOffset now = DateTimeOffset.Now;
            DateTimeOffset nextMerchantsTime = now.AddHours(1).AddMinutes(-26).AddSeconds(-now.Second);

            await textChannel.SendMessageAsync($"Next merchants: <t:{nextMerchantsTime.ToUnixTimeSeconds()}:R>");

            dbcontext.ActiveMerchants.RemoveRange(dbcontext.ActiveMerchants);
            await dbcontext.SaveChangesAsync();

            Program.MerchantMessages = new List<MerchantMessage>();
        }
    }
}