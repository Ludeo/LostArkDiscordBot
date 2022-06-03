﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class EditMessageModule
    {
        public static async Task EditMessageAsync(SocketSlashCommand command)
        {
            if(command.Channel.GetChannelType() != ChannelType.PublicThread) {
                await command.RespondAsync(text: "This command is only available in the thread of the lfg event", ephemeral: true);

                return;
            }

            string customMessage = command.Data.Options.First(x => x.Name == "custom-message").Value.ToString();

            List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
            ThreadLinkedMessage linkedMessage = threadLinkedMessageList.First(x => x.ThreadId == command.Channel.Id);
            ulong messageId = linkedMessage.MessageId;

            ITextChannel channel = Program.Client.GetChannel(Config.Default.LfgChannel) as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(messageId);
            IUserMessage message = messageRaw as IUserMessage;
            ulong authorId = message.Interaction.User.Id;

            if (command.User.Id != authorId)
            {
                await command.RespondAsync(text: "Only the Author of the Event can change the custom message", ephemeral: true);

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

            if (originalEmbed.Timestamp != null)
            {
                newEmbed.Timestamp = originalEmbed.Timestamp.Value;
            }

            newEmbed.AddField("Custom Message", customMessage, false);

            foreach (EmbedField field in originalEmbed.Fields)
            {
                if (field.Name == "Custom Message")
                {
                    continue;
                }

                newEmbed.AddField(field.Name, field.Value, field.Inline);
            }

            await message.ModifyAsync(x => x.Embed = newEmbed.Build());

            try
            {
                await command.RespondAsync(text: "Custom Message updated", ephemeral: true);
            }
            catch (HttpException exception)
            {
                await Program.Log(new LogMessage(LogSeverity.Error, "EditMessageModule.cs", exception.Message));
            }
        }
    }
}