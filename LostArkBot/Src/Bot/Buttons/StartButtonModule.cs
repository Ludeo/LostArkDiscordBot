using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace LostArkBot.Bot.Buttons;

public class StartButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("startbutton")]
    public async Task Start()
    {
        await this.DeferAsync(true);

        IMessage message;
        SocketGuild guild;

        if (this.Context.Channel.GetChannelType() == ChannelType.PublicThread)
        {
            SocketThreadChannel threadChannel = this.Context.Channel as SocketThreadChannel;
            ITextChannel textChannel = threadChannel.ParentChannel as ITextChannel;
            message = await textChannel.GetMessageAsync(threadChannel.Id);
            guild = threadChannel.Guild;
        }
        else
        {
            message = this.Context.Interaction.Message;
            guild = this.Context.Guild;
        }

        Embed originalEmbed;

        if (this.Context.Channel.GetChannelType() == ChannelType.PublicThread)
        {
            originalEmbed = message.Embeds.First() as Embed;
        }
        else
        {
            originalEmbed = this.Context.Interaction.Message.Embeds.First();
        }

        if (!originalEmbed.Fields.Any(x => x.Value.Contains(guild.GetUser(this.Context.User.Id).Mention)))
        {
            await this.FollowupAsync("You are not part of the LFG so you can't start the event", ephemeral: true);

            return;
        }

        if (originalEmbed.Fields.Length is not 0)
        {
            bool skip = false;

            switch (originalEmbed.Fields.Length)
            {
                case 1:
                {
                    if (originalEmbed.Fields.First().Name is "Custom Message" or "Time")
                    {
                        skip = true;
                    }

                    break;
                }
                case 2:
                {
                    if (originalEmbed.Fields.Any(x => x.Name == "Custom Message")
                     && originalEmbed.Fields.Any(x => x.Name == "Time"))
                    {
                        skip = true;
                    }

                    break;
                }
            }

            if (!skip)
            {
                List<string> userMentions = (from embedField in originalEmbed.Fields
                                             where embedField.Name is not ("Custom Message" or "Time")
                                             select embedField.Value.Split("\n")[0]).ToList();

                string pingMessage = userMentions.Aggregate("Event has started!\n", (current, playerMention) => current + playerMention + "\n");

                if (this.Context.Channel.GetChannelType() == ChannelType.PublicThread)
                {
                    await this.Context.Channel.SendMessageAsync(pingMessage);
                }
                else
                {
                    IThreadChannel threadChannel = this.Context.Guild.GetChannel(this.Context.Interaction.Message.Id) as IThreadChannel;
                    await threadChannel.SendMessageAsync(pingMessage);
                }

                return;
            }

            await this.FollowupAsync("This event doesn't have participants", ephemeral: true);

            return;
        }

        await this.FollowupAsync("This event doesn't have participants", ephemeral: true);
    }
}