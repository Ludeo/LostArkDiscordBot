using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class EditTimeModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("edittime", "Edits the time of the LFG")]
        public async Task EditTime([Summary("time", "New time for the event, Format: DD/MM hh:mm")] string time)
        {
            if (Context.Channel.GetChannelType() != ChannelType.PublicThread)
            {
                await RespondAsync(text: "This command is only available in the thread of the lfg event", ephemeral: true);

                return;
            }

            List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
            ThreadLinkedMessage linkedMessage = threadLinkedMessageList.First(x => x.ThreadId == Context.Channel.Id);
            ulong messageId = linkedMessage.MessageId;

            ITextChannel channel = Context.Client.GetChannel(linkedMessage.ChannelId) as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(messageId);
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

            DateTimeOffset now = DateTimeOffset.Now;
            int day = int.Parse(time[..2]);
            int month = int.Parse(time.Substring(3, 2));
            int hour = int.Parse(time.Substring(6, 2));
            int minute = int.Parse(time.Substring(9, 2));
            int year = now.Year;

            if (month < now.Month)
            {
                year += 1;
            }

            DateTimeOffset newDateTime = new(year, month, day, hour, minute, 0, now.Offset);

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
