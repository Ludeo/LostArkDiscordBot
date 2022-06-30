using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Globalization;
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

            if (DateTime.TryParseExact(time, "dd/MM HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime dtParsed))
            {
                int year = DateTimeOffset.Now.Year;

                if (dtParsed.Month < DateTimeOffset.Now.Month)
                {
                    year += 1;
                }

                dtParsed = dtParsed.AddYears(year - dtParsed.Year);
                DateTimeOffset newDateTime = new (dtParsed, new TimeSpan(1, 0, 0));

                EmbedField timeField = originalEmbed.Fields.Where(x => x.Name == "Time").SingleOrDefault();
                newEmbed.AddField("Time", $"<t:{newDateTime.ToUnixTimeSeconds()}:F>\n<t:{newDateTime.ToUnixTimeSeconds()}:R>", timeField.Inline);

                foreach (EmbedField field in originalEmbed.Fields)
                {
                    if (field.Name == "Time") continue;
                    newEmbed.AddField(field.Name, field.Value, field.Inline);
                }

                await message.ModifyAsync(x => x.Embed = newEmbed.Build());
                await Context.Channel.SendMessageAsync(text: "@everyone");
                await RespondAsync(text: $"Time updated to: <t:{newDateTime.ToUnixTimeSeconds()}:F>");

                return;
            }

            await RespondAsync(text: "Wrong time format: Use dd/MM HH:mm", ephemeral: true);

        }
    }
}