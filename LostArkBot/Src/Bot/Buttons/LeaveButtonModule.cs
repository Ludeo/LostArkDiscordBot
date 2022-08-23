using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace LostArkBot.Bot.Buttons;

public class LeaveButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("leavebutton")]
    public async Task Leave()
    {
        await this.DeferAsync();

        Embed originalEmbed;
        SocketThreadChannel threadChannel;
        string userMention = this.Context.User.Mention;

        if (this.Context.Channel.GetChannelType() == ChannelType.PublicThread)
        {
            threadChannel = this.Context.Channel as SocketThreadChannel;
            ITextChannel textChannel = threadChannel.ParentChannel as ITextChannel;
            IMessage message = await textChannel.GetMessageAsync(threadChannel.Id);
            originalEmbed = message.Embeds.First() as Embed;
        }
        else
        {
            threadChannel = this.Context.Guild.GetChannel(this.Context.Interaction.Message.Id) as SocketThreadChannel;
            originalEmbed = this.Context.Interaction.Message.Embeds.First();
        }

        EmbedBuilder newEmbed = new()
        {
            Title = originalEmbed.Title,
            Description = originalEmbed.Description,
            Author = new EmbedAuthorBuilder
            {
                Name = originalEmbed.Author!.Value.Name,
                IconUrl = originalEmbed.Author!.Value.IconUrl,
            },
            ThumbnailUrl = originalEmbed.Thumbnail!.Value.Url,
            ImageUrl = originalEmbed.Image!.Value.Url,
            Color = originalEmbed.Color!.Value,
        };

        bool userLeft = false;

        foreach (EmbedField embedField in originalEmbed.Fields)
        {
            string content = embedField.Value;

            if (content.Contains(userMention))
            {
                userLeft = true;

                continue;
            }

            if (embedField.Name.Contains("Custom Message")
             || embedField.Name.Contains("Time"))
            {
                newEmbed.AddField(embedField.Name, embedField.Value);
            }
            else
            {
                newEmbed.AddField(embedField.Name, embedField.Value, true);
            }
        }

        if (userLeft)
        {
            string title = originalEmbed.Title;
            string title1 = title.Split("(")[1];
            string title2 = title1.Split(")")[0];
            string playerNumberJoined = title2.Split("/")[0];
            string playerNumberMax = title2.Split("/")[1];
            newEmbed.Title = $"{title.Split("(")[0]}({int.Parse(playerNumberJoined) - 1}/{playerNumberMax})";

            if (this.Context.Channel.GetChannelType() == ChannelType.PublicThread)
            {
                ITextChannel textChannel = threadChannel.ParentChannel as ITextChannel;
                IUserMessage message = await textChannel.GetMessageAsync(threadChannel.Id) as IUserMessage;
                await message.ModifyAsync(x => x.Embed = newEmbed.Build());
            }
            else
            {
                await this.ModifyOriginalResponseAsync(x => x.Embed = newEmbed.Build());
            }

            await threadChannel.RemoveUserAsync(this.Context.User as IGuildUser);
        }
        else
        {
            await this.FollowupAsync("You are not part of this event", ephemeral: true);
        }
    }
}