using Discord;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class EditTimeModule
    {
        public static async Task EditTimeAsync(SocketSlashCommand command)
        {
            if (command.Channel.GetChannelType() != ChannelType.PublicThread)
            {
                await command.RespondAsync(text: "This command is only available in the thread of the lfg event", ephemeral: true);

                return;
            }

            string time = command.Data.Options.First(x => x.Name == "time").Value.ToString();

            List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
            ThreadLinkedMessage linkedMessage = threadLinkedMessageList.First(x => x.ThreadId == command.Channel.Id);
            ulong messageId = linkedMessage.MessageId;

            ITextChannel channel = Program.Client.GetChannel(Config.Default.LfgChannel) as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(messageId);
            IUserMessage message = messageRaw as IUserMessage;
            ulong authorId = message.Interaction.User.Id;

            if (command.User.Id != authorId && !Program.Client.GetGuild(Config.Default.Server).GetUser(command.User.Id).GuildPermissions.ManageMessages)
            {
                await command.RespondAsync(text: "Only the Author of the Event can change the time", ephemeral: true);

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

            foreach (EmbedField field in originalEmbed.Fields)
            {
                string value = field.Value;

                if(field.Name == "Time")
                {
                    DateTimeOffset now = DateTimeOffset.Now;
                    int day = int.Parse(time[..2]);
                    int month = int.Parse(time.Substring(3, 2));
                    int hour = int.Parse(time.Substring(6, 2));
                    int minute = int.Parse(time.Substring(9, 2));
                    int year = now.Year;

                    if(month < now.Month)
                    {
                        year += 1;
                    }

                    DateTimeOffset newDateTime = new(year, month, day, hour, minute, 0, now.Offset);

                    value = $"<t:{newDateTime.ToUnixTimeSeconds()}:F>";
                }

                newEmbed.AddField(field.Name, value, field.Inline);
            }

            await message.ModifyAsync(x => x.Embed = newEmbed.Build());

            await command.Channel.SendMessageAsync(text: "Time updated @everyone");

            try
            {
                await command.RespondAsync("\u200b");
            }
            catch (HttpException exception)
            {
                await Program.Log(new LogMessage(LogSeverity.Error, "EditTimeModule.cs", exception.Message));
            }
        }
    }
}
