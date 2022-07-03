using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class LeaveButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("leavebutton")]
        public async Task Leave()
        {
            Embed originalEmbed;
            SocketThreadChannel threadChannel;
            string userMention = Context.User.Mention;

            if(Context.Channel.GetChannelType() == ChannelType.PublicThread)
            {
                threadChannel = Context.Channel as SocketThreadChannel;
                ITextChannel textChannel = threadChannel.ParentChannel as ITextChannel;
                IMessage message = await textChannel.GetMessageAsync(threadChannel.Id);
                originalEmbed = message.Embeds.First() as Embed;
            } else
            {
                threadChannel = Context.Guild.GetChannel(Context.Interaction.Message.Id) as SocketThreadChannel;
                originalEmbed = Context.Interaction.Message.Embeds.First();
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
                ThumbnailUrl = originalEmbed.Thumbnail.Value.Url,
                ImageUrl = originalEmbed.Image.Value.Url,
                Color = originalEmbed.Color.Value,
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
                if (embedField.Name.Contains("Custom Message") || embedField.Name.Contains("Time"))
                {
                    newEmbed.AddField(embedField.Name, embedField.Value, false);
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

                if(Context.Channel.GetChannelType() == ChannelType.PublicThread)
                {
                    ITextChannel textChannel = threadChannel.ParentChannel as ITextChannel;
                    IUserMessage message = await textChannel.GetMessageAsync(threadChannel.Id) as IUserMessage;
                    await message.ModifyAsync(x => x.Embed = newEmbed.Build());

                    try
                    {
                        await RespondAsync();
                    } catch(HttpException exception)
                    {
                        await LogService.Log(new LogMessage(LogSeverity.Error, "LeaveButtonModule.cs", exception.Message));
                    }
                    
                } else
                {
                    await Context.Interaction.UpdateAsync(x => x.Embed = newEmbed.Build());
                }

                await threadChannel.RemoveUserAsync(Context.User as IGuildUser);
            } else
            {
                await RespondAsync(text: "You are not part of this event", ephemeral: true);
            }
        }
    }
}