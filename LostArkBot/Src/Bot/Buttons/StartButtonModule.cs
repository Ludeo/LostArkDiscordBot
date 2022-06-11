using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Buttons
{
    public class StartButtonModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("startbutton")]
        public async Task Start()
        {
            if (Context.User.Id == Context.Interaction.Message.Interaction.User.Id || Context.Guild.GetUser(Context.User.Id).GuildPermissions.ManageMessages)
            {
                Embed originalEmbed;

                if(Context.Channel.GetChannelType() == ChannelType.PublicThread)
                {
                    SocketThreadChannel threadChannel = Context.Channel as SocketThreadChannel;
                    ITextChannel lfgChannel = threadChannel.ParentChannel as ITextChannel;
                    IMessage message = await lfgChannel.GetMessageAsync(threadChannel.Id);
                    originalEmbed = message.Embeds.First() as Embed;
                } else
                {
                    originalEmbed = originalEmbed = Context.Interaction.Message.Embeds.First();
                }

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

                        if(Context.Channel.GetChannelType() == ChannelType.PublicThread)
                        {
                            await Context.Channel.SendMessageAsync(pingMessage);
                        } else
                        {
                            IThreadChannel threadChannel = Context.Guild.GetChannel(Context.Interaction.Message.Id) as IThreadChannel;
                            await threadChannel.SendMessageAsync(pingMessage);
                        }

                        try
                        {
                            await RespondAsync();
                        } catch(HttpException exception)
                        {
                            await Program.Log(new LogMessage(LogSeverity.Error, "StartButton.cs", exception.Message));
                        }

                        return;
                    } else
                    {
                        await RespondAsync(text: "This event doesn't have participants", ephemeral: true);

                        return;
                    }
                }

                await RespondAsync(text: "This event doesn't have participants", ephemeral: true);

                return;
            }

            await RespondAsync(ephemeral: true, text: "You don't have permissions to start this event!");
        }
    }
}