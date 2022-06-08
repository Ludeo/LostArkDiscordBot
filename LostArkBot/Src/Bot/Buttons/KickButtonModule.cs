using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class KickButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("kickbutton")]
        public async Task Kick()
        {
            if (Context.User.Id == Context.Interaction.Message.Interaction.User.Id || Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                Embed originalEmbed = Context.Interaction.Message.Embeds.First();

                if (originalEmbed.Fields.Length == 0
                    || (originalEmbed.Fields.Length == 1 && (originalEmbed.Fields.First().Name == "Custom Message" || originalEmbed.Fields.First().Name == "Time"))
                    || (originalEmbed.Fields.Length == 2 && originalEmbed.Fields.Any(x => x.Name == "Custom Message") && originalEmbed.Fields.Any(x => x.Name == "Time")))
                {
                    await RespondAsync(text: "There is nobody to kick", ephemeral: true);
                }

                SelectMenuBuilder menu = new SelectMenuBuilder().WithCustomId("kick").WithPlaceholder("Select Player to kick");

                foreach (EmbedField field in originalEmbed.Fields)
                {
                    if (field.Name == "Custom Message" || field.Name == "Time")
                    {
                        continue;
                    }

                    menu.AddOption(field.Value.Split("\n")[1], field.Value.Split("\n")[1]);
                }

                await RespondAsync(components: new ComponentBuilder().WithSelectMenu(menu).Build(), ephemeral: true);

                return;
            }

            await RespondAsync(ephemeral: true, text: "You don't have permissions to delete this event!");
        }
    }
}