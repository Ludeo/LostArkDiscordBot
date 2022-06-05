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

namespace LostArkBot.Src.Bot.Buttons
{
    internal class StartButton
    {
        public static async Task Start(SocketMessageComponent component)
        {
            if (component.User.Id == component.Message.Interaction.User.Id || Program.Client.GetGuild(Config.Default.Server).GetUser(component.User.Id).GuildPermissions.ManageMessages)
            {
                Embed originalEmbed = component.Message.Embeds.First();

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

                        List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
                        ThreadLinkedMessage linkedMessage = threadLinkedMessageList.First(x => x.MessageId == component.Message.Id);

                        IThreadChannel threadChannel = Program.Client.GetChannel(linkedMessage.ThreadId) as IThreadChannel;
                        await threadChannel.SendMessageAsync(pingMessage);

                        try
                        {
                            await component.RespondAsync();
                        } catch(HttpException exception)
                        {
                            await Program.Log(new LogMessage(LogSeverity.Error, "StartButton.cs", exception.Message));
                        }
                    } else
                    {
                        await component.RespondAsync(text: "This event doesn't have participants", ephemeral: true);
                    }
                }

                return;
            }

            await component.RespondAsync(ephemeral: true, text: "You don't have permissions to start this event!");
        }
    }
}