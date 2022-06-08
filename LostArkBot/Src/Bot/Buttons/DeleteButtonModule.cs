using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class DeleteButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("deletebutton")]
        public async Task Delete()
        {
            if (Context.User.Id == Context.Interaction.User.Id || Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
                ThreadLinkedMessage linkedMessage = threadLinkedMessageList.FirstOrDefault(x => x.MessageId == Context.Interaction.Message.Id);

                if(linkedMessage == null)
                {
                    await Context.Interaction.Message.DeleteAsync();

                    return;
                }
                
                threadLinkedMessageList.Remove(linkedMessage);
                File.WriteAllText("ThreadMessageLink.json", JsonSerializer.Serialize(threadLinkedMessageList));

                await Context.Interaction.Message.DeleteAsync();

                try
                {
                    IThreadChannel thread = Context.Guild.GetChannel(linkedMessage.ThreadId) as IThreadChannel;
                    await thread.DeleteAsync();
                } catch(HttpException exception)
                {
                    await Program.Log(new LogMessage(LogSeverity.Error, "DeleteButton.cs", exception.Message));
                }
                
                return;
            }

            await RespondAsync(ephemeral: true, text: "You don't have permissions to delete this event!");
        }
    }
}