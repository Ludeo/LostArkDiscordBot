using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects.LostMerchants;
using LostArkBot.Bot.Shared;
using LostArkBot.databasemodels;
using Quartz;

namespace LostArkBot.Bot.QuartzJobs;

// ReSharper disable once ClassNeverInstantiated.Global
public class MerchantJob : IJob
{
    private readonly LostArkBotContext dbcontext;

    public MerchantJob(LostArkBotContext dbcontext) => this.dbcontext = dbcontext;

    public async Task Execute(IJobExecutionContext context)
    {
        await LogService.Log(LogSeverity.Info, this.GetType().Name, "Executing Quartz job");
        SocketTextChannel textChannel = Program.MerchantChannel;
        List<IMessage> messages = await textChannel.GetMessagesAsync().Flatten().ToListAsync();

        await textChannel.DeleteMessagesAsync(messages);
        DateTimeOffset now = DateTimeOffset.Now;
        DateTimeOffset nextMerchantsTime = now.AddHours(1).AddMinutes(-26).AddSeconds(-now.Second);

        await textChannel.SendMessageAsync($"Next merchants: <t:{nextMerchantsTime.ToUnixTimeSeconds()}:R>");

        this.dbcontext.ActiveMerchants.RemoveRange(this.dbcontext.ActiveMerchants);
        await this.dbcontext.SaveChangesAsync();

        Program.MerchantMessages = new List<MerchantMessage>();
    }
}