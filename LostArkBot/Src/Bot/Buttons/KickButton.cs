using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    internal class KickButton
    {
        public static async Task Kick(SocketMessageComponent component)
        {
            if (component.User.Id == component.Message.Interaction.User.Id || Program.Client.GetGuild(Config.Default.Server).GetUser(component.User.Id).GuildPermissions.ManageMessages)
            {
                Embed originalEmbed = component.Message.Embeds.First();

                if (originalEmbed.Fields.Length == 0
                    || (originalEmbed.Fields.Length == 1 && (originalEmbed.Fields.First().Name == "Custom Message" || originalEmbed.Fields.First().Name == "Time"))
                    || (originalEmbed.Fields.Length == 2 && originalEmbed.Fields.Any(x => x.Name == "Custom Message") && originalEmbed.Fields.Any(x => x.Name == "Time")))
                {
                    await component.RespondAsync(text: "There is nobody to kick", ephemeral: true);
                }

                SelectMenuBuilder menu = new SelectMenuBuilder().WithCustomId("kick").WithPlaceholder("Select Player to kick");

                foreach (EmbedField field in originalEmbed.Fields)
                {
                    if (field.Name == "Custom Message" || field.Name == "Time")
                    {
                        continue;
                    }

                    menu.AddOption(field.Value.Split("\n")[1], $"{component.Message.Id},{field.Value.Split("\n")[1]}");
                }

                await component.RespondAsync(components: new ComponentBuilder().WithSelectMenu(menu).Build(), ephemeral: true);

                return;
            }

            await component.RespondAsync(ephemeral: true, text: "You don't have permissions to delete this event!");
        }
    }
}