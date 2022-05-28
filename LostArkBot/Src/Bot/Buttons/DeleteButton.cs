using Discord;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    internal class DeleteButton
    {
        public static async Task Delete(SocketMessageComponent component)
        {
            if (component.User.Id == component.Message.Interaction.User.Id || Program.Client.GetGuild(Config.Default.Server).GetUser(component.User.Id).GuildPermissions.ManageMessages)
            {
                List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
                ThreadLinkedMessage linkedMessage = threadLinkedMessageList.First(x => x.MessageId == component.Message.Id);
                
                threadLinkedMessageList.Remove(linkedMessage);
                File.WriteAllText("ThreadMessageLink.json", JsonSerializer.Serialize(threadLinkedMessageList));

                await component.Message.DeleteAsync();

                try
                {
                    IThreadChannel thread = Program.Client.GetChannel(linkedMessage.ThreadId) as IThreadChannel;
                    await thread.DeleteAsync();
                } catch(HttpException exception)
                {
                }
                
                return;
            }

            await component.RespondAsync(ephemeral: true, text: "You don't have permissions to delete this event!");
        }
    }
}