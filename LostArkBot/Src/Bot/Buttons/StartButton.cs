using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Collections.Generic;
using System.Linq;
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

                EmbedBuilder newEmbed = new()
                {
                    Title = originalEmbed.Title,
                    Description = "Event has started at " + DateTime.Now,
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

                ActionRowComponent components = component.Message.Components.First();
                ComponentBuilder componentBuilder = new();

                foreach (ButtonComponent button in components.Components)
                {
                    ButtonBuilder newButton = new();
                    if (button.CustomId == "delete")
                    {
                        newButton.IsDisabled = false;
                    }
                    else
                    {
                        newButton.IsDisabled = true;
                    }

                    newButton.CustomId = button.CustomId;
                    newButton.Label = button.Label;
                    newButton.Style = button.Style;

                    componentBuilder.WithButton(newButton);
                }

                foreach (EmbedField embedField in originalEmbed.Fields)
                {
                    EmbedFieldBuilder newEmbedField = new()
                    {
                        IsInline = embedField.Inline,
                        Name = embedField.Name,
                        Value = embedField.Value,
                    };

                    newEmbed.AddField(newEmbedField);
                }

                await component.UpdateAsync(x =>
                {
                    x.Embed = newEmbed.Build();
                    x.Components = componentBuilder.Build();
                });

                if (originalEmbed.Fields.Length is not 0)
                {
                    bool skip = false;
                    if (originalEmbed.Fields.Length is 1)
                    {
                        if (originalEmbed.Fields.First().Name == "Custom Message")
                        {
                            skip = true;
                        }
                    }

                    if (!skip)
                    {
                        List<string> userMentions = new();

                        foreach (EmbedField embedField in originalEmbed.Fields)
                        {
                            if (embedField.Name == "Custom Message")
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

                        await component.FollowupAsync(pingMessage);
                    }
                }

                return;
            }

            await component.RespondAsync(ephemeral: true, text: "You don't have permissions to start this event!");
        }
    }
}