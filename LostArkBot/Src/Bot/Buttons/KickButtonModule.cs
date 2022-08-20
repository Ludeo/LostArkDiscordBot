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
            await DeferAsync(ephemeral: true);

            IMessage message;
            SocketGuild guild;

            if (Context.Channel.GetChannelType() == ChannelType.PublicThread)
            {
                SocketThreadChannel threadChannel = Context.Channel as SocketThreadChannel;
                ITextChannel textChannel = threadChannel.ParentChannel as ITextChannel;
                message = await textChannel.GetMessageAsync(threadChannel.Id);
                guild = threadChannel.Guild;
            }
            else
            {
                message = Context.Interaction.Message;
                guild = Context.Guild;
            }

            ulong authorId = ulong.Parse(message.Embeds.First().Author.Value.Name.Split("\n")[1]);

            if (Context.User.Id != authorId && !guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                await FollowupAsync(ephemeral: true, text: "You don't have permissions to kick users from the event!");
                return;
            }

            Embed originalEmbed = message.Embeds.First() as Embed;

            if (originalEmbed.Fields.Length == 0
                || (originalEmbed.Fields.Length == 1 && (originalEmbed.Fields.First().Name == "Custom Message" || originalEmbed.Fields.First().Name == "Time"))
                || (originalEmbed.Fields.Length == 2 && originalEmbed.Fields.Any(x => x.Name == "Custom Message") && originalEmbed.Fields.Any(x => x.Name == "Time")))
            {
                await FollowupAsync(text: "There is nobody to kick", ephemeral: true);
                return;
            }

            SelectMenuBuilder menu = new SelectMenuBuilder().WithCustomId("kick").WithPlaceholder("Select Player to kick");

            foreach (EmbedField field in originalEmbed.Fields)
            {
                if (field.Name == "Custom Message" || field.Name == "Time")
                {
                    continue;
                }

                string characterName = field.Value.Split("\n")[1];
                string description = characterName;

                if (characterName.ToLower() != "no character".ToLower())
                {
                    string itemLevel = field.Value.Split("\n")[2];
                    string className = field.Value.Split("\n")[3].Split(" ")[1];
                    description += $" - {className}, {itemLevel}";
                }
                menu.AddOption(field.Name, field.Name, description);

            }

            await FollowupAsync(text: "Select the Player you want to kick from the lfg", components: new ComponentBuilder().WithSelectMenu(menu).Build(), ephemeral: true);
            return;
        }
    }
}