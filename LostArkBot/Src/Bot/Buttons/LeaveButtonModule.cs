using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class LeaveButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("leavebutton")]
        public async Task Leave()
        {
            Embed originalEmbed = Context.Interaction.Message.Embeds.First();
            string userMention = Context.User.Mention;

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

                await Context.Interaction.UpdateAsync(x => x.Embed = newEmbed.Build());

                IThreadChannel threadChannel = Context.Guild.GetChannel(Context.Interaction.Message.Id) as IThreadChannel;
                await threadChannel.RemoveUserAsync(Context.User as IGuildUser);
            } else
            {
                await RespondAsync(text: "You are not part of this event", ephemeral: true);
            }
        }
    }
}