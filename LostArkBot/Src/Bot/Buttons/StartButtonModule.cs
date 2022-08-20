using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class StartButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("startbutton")]
        public async Task Start()
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

            Embed originalEmbed;

            if(Context.Channel.GetChannelType() == ChannelType.PublicThread)
            {
                originalEmbed = message.Embeds.First() as Embed;
            } else
            {
                originalEmbed = Context.Interaction.Message.Embeds.First();
            }

            if (originalEmbed.Fields.Length is not 0)
            {
                bool skip = false;
                if (originalEmbed.Fields.Length is 1)
                {
                    if (originalEmbed.Fields.First().Name == "Custom Message" || originalEmbed.Fields.First().Name == "Time")
                    {
                        skip = true;
                    }
                } else if(originalEmbed.Fields.Length is 2)
                {
                    if(originalEmbed.Fields.Any(x => x.Name == "Custom Message") && originalEmbed.Fields.Any(x => x.Name == "Time"))
                    {
                        skip = true;
                    }
                }

                if (!skip)
                {
                    List<string> userMentions = new();

                    foreach (EmbedField embedField in originalEmbed.Fields)
                    {
                        if (embedField.Name == "Custom Message" || embedField.Name == "Time")
                        {
                            continue;
                        }

                        string playerMention = embedField.Value.Split("\n")[0];
                        userMentions.Add(playerMention);
                    }

                    string pingMessage = "Event has started!\n";

                    foreach (string playerMention in userMentions)
                    {
                        pingMessage += playerMention + "\n";
                    }

                    if(Context.Channel.GetChannelType() == ChannelType.PublicThread)
                    {
                        await Context.Channel.SendMessageAsync(pingMessage);
                    } else
                    {
                        IThreadChannel threadChannel = Context.Guild.GetChannel(Context.Interaction.Message.Id) as IThreadChannel;
                        await threadChannel.SendMessageAsync(pingMessage);
                    }

                    return;
                } else
                {
                    await FollowupAsync(text: "This event doesn't have participants", ephemeral: true);

                    return;
                }
            }

            await FollowupAsync(text: "This event doesn't have participants", ephemeral: true);
        }
    }
}