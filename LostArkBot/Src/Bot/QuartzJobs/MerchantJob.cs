using Discord;
using Discord.WebSocket;
using Quartz;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.QuartzJobs
{
    public class MerchantJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            SocketTextChannel textChannel = Program.MerchantChannel;
            IAsyncEnumerable<IMessage> messages = textChannel.GetMessagesAsync().Flatten();
            List<IMessage> messageList = new();

            await foreach (IMessage message in messages)
            {
                messageList.Add(message);
            }
            await textChannel.DeleteMessagesAsync(messageList);
        }
    }
}