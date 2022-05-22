using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    internal class DeleteButton
    {
        public static async Task Delete(SocketMessageComponent component)
        {
            if (component.User.Id == component.Message.Interaction.User.Id || Program.Client.GetGuild(Config.Default.Server).GetUser(component.User.Id).GuildPermissions.ManageMessages)
            {
                await component.Message.DeleteAsync();
                return;
            }

            await component.RespondAsync(ephemeral: true, text: "You don't have permissions to delete this event!");
        }
    }
}