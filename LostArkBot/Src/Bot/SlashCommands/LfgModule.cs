using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using LostArkBot.databasemodels;
using Microsoft.EntityFrameworkCore;

namespace LostArkBot.Bot.SlashCommands;

[Group("lfg", "Create a LFG or manage it")]
public class LfgModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    private readonly LostArkBotContext dbcontext;

    public LfgModule(LostArkBotContext dbcontext) => this.dbcontext = dbcontext;

    [SlashCommand("adduser", "Adds a user to the lfg")]
    public async Task AddUser([Summary("character-name", "Name of the character that you want to add")] string characterName)
    {
        await this.DeferAsync(true);

        if (this.Context.Channel.GetChannelType() != ChannelType.PublicThread)
        {
            await this.FollowupAsync("This command is only available in the thread of the lfg event", ephemeral: true);

            return;
        }

        SocketThreadChannel threadChannel = this.Context.Channel as SocketThreadChannel;
        ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
        IMessage messageRaw = await channel.GetMessageAsync(threadChannel.Id);
        IUserMessage message = messageRaw as IUserMessage;

        ulong authorId = ulong.Parse(messageRaw.Embeds.First().Author!.Value.Name.Split("\n")[1]);

        if (this.Context.User.Id != authorId
         && !this.Context.Guild.GetUser(this.Context.User.Id).GuildPermissions.ManageMessages)
        {
            await this.FollowupAsync("You don't have permissions to delete this event!", ephemeral: true);

            return;
        }

        Character character = this.dbcontext.Characters.Include(x => x.User).FirstOrDefault(x => x.CharacterName == characterName);

        if (character is null)
        {
            await this.FollowupAsync("This character does not exist", ephemeral: true);

            return;
        }

        Embed originalEmbed = message.Embeds.First() as Embed;
        SocketGuildUser guildUser = this.Context.Guild.GetUser(character.User.DiscordUserId);

        if (originalEmbed.Fields.Any(x => x.Value.Contains(guildUser.Mention)))
        {
            await this.FollowupAsync("This user is already part of the LFG", ephemeral: true);

            return;
        }

        List<GuildEmote> emotes = Program.GuildEmotes;
        GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

        string title = originalEmbed.Title;
        string title1 = title.Split("(")[1];
        string title2 = title1.Split(")")[0];
        int playerNumberJoined = int.Parse(title2.Split("/")[0]) + 1;
        string playerNumberMax = title2.Split("/")[1];

        EmbedBuilder embed = new()
        {
            Title = $"{title.Split("(")[0]}({playerNumberJoined}/{playerNumberMax})",
            Description = originalEmbed.Description,
            Author = new EmbedAuthorBuilder
            {
                Name = originalEmbed.Author!.Value.Name,
                IconUrl = originalEmbed.Author!.Value.IconUrl,
            },
            ThumbnailUrl = originalEmbed.Thumbnail!.Value.Url,
            ImageUrl = originalEmbed.Image!.Value.Url,
            Color = originalEmbed.Color!.Value,
        };

        foreach (EmbedField field in originalEmbed.Fields)
        {
            embed.AddField(field.Name, field.Value, field.Inline);
        }

        embed.AddField(
                       guildUser.DisplayName,
                       $"{guildUser.Mention}\n{character.CharacterName}\n{character.ItemLevel}\n"
                     + $"<:{emote.Name}:{emote.Id}> {character.ClassName}",
                       true);

        await message.ModifyAsync(x => x.Embed = embed.Build());
        await threadChannel.AddUserAsync(guildUser);
        await this.FollowupAsync(characterName + " got successfully added to the LFG", ephemeral: true);
    }

    [SlashCommand("controls", "Posts the buttons of the lfg")]
    public async Task Controls()
    {
        await this.DeferAsync();

        if (this.Context.Channel.GetChannelType() != ChannelType.PublicThread)
        {
            IMessage message = await this.FollowupAsync("auto-delete");
            await message.DeleteAsync();
            await this.FollowupAsync("This command can only be executed in the thread channel of the lfg", ephemeral: true);

            return;
        }

        ComponentBuilder components = new ComponentBuilder().WithButton(Program.StaticObjects.JoinButton)
                                                            .WithButton(Program.StaticObjects.LeaveButton)
                                                            .WithButton(Program.StaticObjects.KickButton)
                                                            .WithButton(Program.StaticObjects.DeleteButton)
                                                            .WithButton(Program.StaticObjects.StartButton);

        await this.FollowupAsync(components: components.Build());
    }

    [SlashCommand("create", "Creates an LFG event")]
    public async Task Create(
        [Summary("custom-message", "Custom Message that will be displayed in the LFG")] string customMessage = "",
        [Summary("time", "Time of the LFG, must be server time and must have format: DD/MM hh:mm")]
        string time = "")
    {
        await this.DeferAsync(true);

        if (this.Context.Channel.GetChannelType() == ChannelType.PublicThread)
        {
            await this.FollowupAsync("This command can only be used in a text channel", ephemeral: true);

            return;
        }

        ComponentBuilder component =
            new ComponentBuilder().WithSelectMenu(Program.StaticObjects.HomeLfg).WithButton(Program.StaticObjects.DeleteButton);

        EmbedBuilder embed = new()
        {
            Title = "Creating a LFG Event",
            Description = "Select the Event from the menu that you would like to create",
            Color = Color.Gold,
        };

        if (!string.IsNullOrEmpty(customMessage))
        {
            embed.Footer = new EmbedFooterBuilder
            {
                Text = customMessage,
            };
        }

        if (!string.IsNullOrEmpty(time))
        {
            if (DateTime.TryParseExact(time, "dd/MM HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime dtParsed))
            {
                int year = DateTimeOffset.Now.Year;

                if (dtParsed.Month < DateTimeOffset.Now.Month)
                {
                    year += 1;
                }

                dtParsed = dtParsed.AddYears(year - dtParsed.Year);

                embed.Timestamp = new DateTimeOffset(dtParsed, StaticObjects.TimeOffset);
            }
        }

        await this.FollowupAsync(embed: embed.Build(), components: component.Build(), ephemeral: true);
    }

    [SlashCommand("editmessage", "Edits the message of the LFG")]
    public async Task EditMessage([Summary("custom-message", "New custom message for the event")] string customMessage)
    {
        await this.DeferAsync(true);

        if (this.Context.Channel.GetChannelType() != ChannelType.PublicThread)
        {
            await this.FollowupAsync("This command is only available in the thread of the lfg event", ephemeral: true);

            return;
        }

        SocketThreadChannel threadChannel = this.Context.Channel as SocketThreadChannel;
        ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
        IMessage messageRaw = await channel.GetMessageAsync(threadChannel.Id);
        IUserMessage message = messageRaw as IUserMessage;
        ulong authorId = ulong.Parse(message.Embeds.First().Author!.Value.Name.Split("\n")[1]);

        if (this.Context.User.Id != authorId
         && !this.Context.Guild.GetUser(this.Context.User.Id).GuildPermissions.ManageMessages)
        {
            await this.FollowupAsync("Only the Author of the Event can change the custom message", ephemeral: true);

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
            ThumbnailUrl = originalEmbed.Thumbnail!.Value.Url,
            ImageUrl = originalEmbed.Image!.Value.Url,
            Color = originalEmbed.Color!.Value,
        };

        if (originalEmbed.Timestamp != null)
        {
            newEmbed.Timestamp = originalEmbed.Timestamp.Value;
        }

        newEmbed.AddField("Custom Message", customMessage);

        foreach (EmbedField field in originalEmbed.Fields.Where(field => field.Name != "Custom Message"))
        {
            newEmbed.AddField(field.Name, field.Value, field.Inline);
        }

        await message.ModifyAsync(x => x.Embed = newEmbed.Build());
        await this.FollowupAsync("Custom Message updated", ephemeral: true);
    }

    [SlashCommand("edittime", "Edits the time of the LFG")]
    public async Task EditTime([Summary("time", "New time of the LFG, must be server time and must have format: DD/MM hh:mm")] string time)
    {
        await this.DeferAsync();

        if (this.Context.Channel.GetChannelType() != ChannelType.PublicThread)
        {
            IMessage deleteMessage = await this.FollowupAsync("auto-delete");
            await deleteMessage.DeleteAsync();
            await this.FollowupAsync("This command is only available in the thread of the lfg event", ephemeral: true);

            return;
        }

        SocketThreadChannel threadChannel = this.Context.Channel as SocketThreadChannel;
        ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
        IMessage messageRaw = await channel.GetMessageAsync(threadChannel.Id);
        IUserMessage message = messageRaw as IUserMessage;
        ulong authorId = ulong.Parse(message.Embeds.First().Author!.Value.Name.Split("\n")[1]);

        if (this.Context.User.Id != authorId
         && !this.Context.Guild.GetUser(this.Context.User.Id).GuildPermissions.ManageMessages)
        {
            IMessage deleteMessage = await this.FollowupAsync("auto-delete");
            await deleteMessage.DeleteAsync();
            await this.FollowupAsync("Only the Author of the Event can change the time", ephemeral: true);

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
            ThumbnailUrl = originalEmbed.Thumbnail!.Value.Url,
            ImageUrl = originalEmbed.Image!.Value.Url,
            Color = originalEmbed.Color!.Value,
        };

        if (DateTime.TryParseExact(time, "dd/MM HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime dtParsed))
        {
            int year = DateTimeOffset.Now.Year;

            if (dtParsed.Month < DateTimeOffset.Now.Month)
            {
                year += 1;
            }

            dtParsed = dtParsed.AddYears(year - dtParsed.Year);
            DateTimeOffset newDateTime = new(dtParsed, StaticObjects.TimeOffset);

            EmbedField timeField = originalEmbed.Fields.SingleOrDefault(x => x.Name == "Time");
            newEmbed.AddField("Time", $"<t:{newDateTime.ToUnixTimeSeconds()}:F>\n<t:{newDateTime.ToUnixTimeSeconds()}:R>", timeField.Inline);

            foreach (EmbedField field in originalEmbed.Fields.Where(field => field.Name != "Time"))
            {
                newEmbed.AddField(field.Name, field.Value, field.Inline);
            }

            await message.ModifyAsync(x => x.Embed = newEmbed.Build());
            await this.Context.Channel.SendMessageAsync("@everyone");
            await this.FollowupAsync($"Time updated to: <t:{newDateTime.ToUnixTimeSeconds()}:F>");

            return;
        }

        IMessage deleteMessage2 = await this.FollowupAsync("auto-delete");
        await deleteMessage2.DeleteAsync();
        await this.FollowupAsync("Wrong time format: Use dd/MM HH:mm", ephemeral: true);
    }

    [SlashCommand("rename-thread", "Renames the thread of the LFG")]
    public async Task RenameThread([Summary("name", "New name for the thread")] string name)
    {
        await this.DeferAsync(true);

        if (this.Context.Channel.GetChannelType() != ChannelType.PublicThread)
        {
            await this.FollowupAsync("This command is only available in the thread of the lfg event", ephemeral: true);

            return;
        }

        SocketThreadChannel threadChannel = this.Context.Channel as SocketThreadChannel;
        ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
        IMessage messageRaw = await channel.GetMessageAsync(threadChannel.Id);
        IUserMessage message = messageRaw as IUserMessage;
        ulong authorId = ulong.Parse(message.Embeds.First().Author!.Value.Name.Split("\n")[1]);

        if (this.Context.User.Id != authorId
         && !this.Context.Guild.GetUser(this.Context.User.Id).GuildPermissions.ManageMessages)
        {
            await this.FollowupAsync("Only the Author of the Event can change the time", ephemeral: true);

            return;
        }

        await threadChannel.ModifyAsync(x => x.Name = name);
        await this.FollowupAsync("Successfully renamed the name of the thread", ephemeral: true);
    }

    [SlashCommand("when", "Shows the time of the lfg if set")]
    public async Task When()
    {
        await this.DeferAsync();

        if (this.Context.Channel.GetChannelType() != ChannelType.PublicThread)
        {
            IMessage deleteMessage = await this.FollowupAsync("auto-delete");
            await deleteMessage.DeleteAsync();
            await this.FollowupAsync("This command is only available in the thread of the lfg event", ephemeral: true);

            return;
        }

        SocketThreadChannel threadChannel = this.Context.Channel as SocketThreadChannel;
        ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
        IMessage messageRaw = await channel.GetMessageAsync(threadChannel.Id);
        IUserMessage message = messageRaw as IUserMessage;

        Embed embed = message.Embeds.First() as Embed;
        EmbedField timeField = new();

        foreach (EmbedField field in embed.Fields.Where(field => field.Name == "Time"))
        {
            timeField = field;
        }

        if (string.IsNullOrEmpty(timeField.Name))
        {
            IMessage deleteMessage = await this.FollowupAsync("auto-delete");
            await deleteMessage.DeleteAsync();
            await this.RespondAsync("This event doesn't have a time set", ephemeral: true);

            return;
        }

        string time = timeField.Value.Split("\n")[0];
        long unixSeconds = long.Parse(time.Replace("<t:", "").Replace(":F>", ""));

        await this.FollowupAsync($"The event starts at <t:{unixSeconds}:F>\n\nThat's <t:{unixSeconds}:R>");
    }
}