using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class EditTimeModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("edittime", "Edits the time of the LFG")]
        public async Task EditTime([Summary("time", "New time of the LFG, must be server time and must have format: DD/MM hh:mm")] string time)
        {
            if (Context.Channel.GetChannelType() != ChannelType.PublicThread)
            {
                await RespondAsync(text: "This command is only available in the thread of the lfg event", ephemeral: true);

                return;
            }

            SocketThreadChannel threadChannel = Context.Channel as SocketThreadChannel;
            ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(threadChannel.Id);
            IUserMessage message = messageRaw as IUserMessage;
            ulong authorId = message.Interaction.User.Id;

            if (Context.User.Id != authorId && !Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                await RespondAsync(text: "Only the Author of the Event can change the time", ephemeral: true);

                return;
            }

            Embed originalEmbed = message.Embeds.First() as Embed;

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

            int day = int.Parse(time[..2]);
            int month = int.Parse(time.Substring(3, 2));
            int hour = int.Parse(time.Substring(6, 2));
            int minute = int.Parse(time.Substring(9, 2));
            int year = DateTimeOffset.Now.Year;

            if (month < DateTimeOffset.Now.Month)
            {
                year += 1;
            }

            DateTimeOffset newDateTime = new(year, month, day, hour, minute, DateTimeOffset.Now.Second, new TimeSpan(1, 0, 0));

            if (!originalEmbed.Fields.Any(x => x.Name == "Time"))
            {
                newEmbed.AddField("Time", $"<t:{newDateTime.ToUnixTimeSeconds()}:F>");
            }

            foreach (EmbedField field in originalEmbed.Fields)
            {
                string value = field.Value;

                if (field.Name == "Time")
                {
                    value = $"<t:{newDateTime.ToUnixTimeSeconds()}:F>";
                }

                newEmbed.AddField(field.Name, value, field.Inline);
            }

            await message.ModifyAsync(x => x.Embed = newEmbed.Build());

            await RespondAsync(text: "Time updated");
            await Context.Channel.SendMessageAsync(text: "@everyone");
        }
    }
}
