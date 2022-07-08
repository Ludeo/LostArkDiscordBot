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
            IMessage message;
            SocketGuild guild;

            if(Context.Channel.GetChannelType() == ChannelType.PublicThread)
            {
                SocketThreadChannel threadChannel = Context.Channel as SocketThreadChannel;
                ITextChannel textChannel = threadChannel.ParentChannel as ITextChannel;
                message = await textChannel.GetMessageAsync(threadChannel.Id);
                guild = threadChannel.Guild;
            } else
            {
                message = Context.Interaction.Message;
                guild = Context.Guild;
            }

            if (Context.User.Id == message.Interaction.User.Id || guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                Embed originalEmbed = message.Embeds.First() as Embed;

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

            await RespondAsync(ephemeral: true, text: "You don't have permissions to kick users from the event!");
        }
    }
}