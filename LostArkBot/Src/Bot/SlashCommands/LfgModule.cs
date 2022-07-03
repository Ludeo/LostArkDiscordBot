using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [Group("lfg", "Create a LFG or manage it")]
    public class LfgModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("create", "Creates an LFG event")]
        public async Task Create(
            [Summary("custom-message", "Custom Message that will be displayed in the LFG")] string customMessage = "",
            [Summary("time", "Time of the LFG, must be server time and must have format: DD/MM hh:mm")] string time = "",
            [Summary("static-group", "Name of the static group")] string staticGroup = "")
        {
            if (!string.IsNullOrEmpty(staticGroup))
            {
                List<StaticGroup> staticGroups = JsonSerializer.Deserialize<List<StaticGroup>>(File.ReadAllText("staticgroups.json"));

                if (!staticGroups.Any(x => x.Name == staticGroup))
                {
                    await RespondAsync(text: "The given static group doesn't exist", ephemeral: true);
                    return;
                }
            }

            if (Context.Channel.GetChannelType() == ChannelType.PublicThread)
            {
                await RespondAsync(text: "This command can only be used in a text channel", ephemeral: true);
                return;
            }

            ComponentBuilder component = new ComponentBuilder().WithSelectMenu(Program.StaticObjects.HomeLfg).WithButton(Program.StaticObjects.DeleteButton);

            EmbedBuilder embed = new()
            {
                Title = "Creating a LFG Event",
                Description = "Select the Event from the menu that you would like to create",
                Color = Color.Gold,
            };

            if (!string.IsNullOrEmpty(customMessage))
            {
                embed.Footer = new EmbedFooterBuilder()
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

                    embed.Timestamp = new DateTimeOffset(dtParsed, new TimeSpan(1, 0, 0));
                }
            }

            await RespondAsync(text: staticGroup, embed: embed.Build(), components: component.Build());
        }

        [SlashCommand("adduser", "Adds a user to the lfg")]
        public async Task AddUser([Summary("character-name", "Name of the character that you want to add")] string characterName)
        {
            if (Context.Channel.GetChannelType() != ChannelType.PublicThread)
            {
                await RespondAsync(text: "This command is only available in the thread of the lfg event", ephemeral: true);
                return;
            }

            List<Character> characters = JsonSerializer.Deserialize<List<Character>>(File.ReadAllText("characters.json"));

            if (!characters.Any(x => x.CharacterName == characterName))
            {
                await RespondAsync(text: "This character does not exist", ephemeral: true);
                return;
            }

            Character character = characters.Find(x => x.CharacterName == characterName);

            SocketThreadChannel threadChannel = Context.Channel as SocketThreadChannel;
            ITextChannel channel = threadChannel.ParentChannel as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(threadChannel.Id);
            IUserMessage message = messageRaw as IUserMessage;

            Embed originalEmbed = message.Embeds.First() as Embed;
            SocketGuildUser user = Context.Guild.GetUser(character.DiscordUserId);

            if (originalEmbed.Fields.Any(x => x.Value.Contains(user.Mention)))
            {
                await RespondAsync(text: "This user is already part of the LFG", ephemeral: true);
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
                ThumbnailUrl = originalEmbed.Thumbnail.Value.Url,
                ImageUrl = originalEmbed.Image.Value.Url,
                Color = originalEmbed.Color.Value,
            };

            foreach (EmbedField field in originalEmbed.Fields)
            {
                embed.AddField(field.Name, field.Value, field.Inline);
            }

            embed.AddField(user.DisplayName + " has joined",
                                    $"{user.Mention}\n{character.CharacterName}\n{character.ItemLevel}\n"
                                    + $"<:{emote.Name}:{emote.Id}> {character.ClassName}",
                                    true);

            await message.ModifyAsync(x => x.Embed = embed.Build());
            await threadChannel.AddUserAsync(user);
            await RespondAsync(text: characterName + " got successfully added to the LFG", ephemeral: true);
        }

        [SlashCommand("calendar", "Exports the date of the event as a ics file so you can import it into your calendar")]
        public async Task Calendar(
            [Summary("duration", "The duration of the event in hours")] int duration = 2)
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

            Embed originalEmbed = message.Embeds.First() as Embed;
            string time = string.Empty;

            foreach (EmbedField field in originalEmbed.Fields)
            {
                if (field.Name == "Time")
                {
                    time = field.Value.Split("\n")[0];
                }
            }

            if (string.IsNullOrEmpty(time))
            {
                await RespondAsync(text: "This lfg doesn't have a time set", ephemeral: true);
            }

            long unixSeconds = long.Parse(time.Replace("<t:", "").Replace(":F>", ""));
            DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

            string timeStartFormatted = date.ToString("yyyyMMddTHHmmssZ");
            date = date.AddHours(duration);
            string timeEndFormatted = date.ToString("yyyyMMddTHHmmssZ");
            string summary = threadChannel.Name;

            string icsString = "BEGIN:VCALENDAR\nVERSION:2.0\nPRODID:-//Ludeo//Lost Ark Bot//EN\nBEGIN:VEVENT\nDTSTART:" + timeStartFormatted + "\nDTEND:" + timeEndFormatted
                + "\nSUMMARY:" + summary + "\nEND:VEVENT\nEND:VCALENDAR";

            await File.WriteAllTextAsync("DateExport.ics", icsString);
            await RespondWithFileAsync(fileStream: File.OpenRead("DateExport.ics"), fileName: "DateExport.ics", ephemeral: true);
            File.Delete("DateExport.ics");
        }

        [SlashCommand("controls", "Posts the buttons of the lfg")]
        public async Task Controls()
        {
            if (Context.Channel.GetChannelType() != ChannelType.PublicThread)
            {
                await RespondAsync(text: "This command can only be executed in the thread channel of the lfg", ephemeral: true);
                return;
            }

            ComponentBuilder components = new ComponentBuilder().WithButton(Program.StaticObjects.JoinButton)
                                                                .WithButton(Program.StaticObjects.LeaveButton)
                                                                .WithButton(Program.StaticObjects.KickButton)
                                                                .WithButton(Program.StaticObjects.DeleteButton)
                                                                .WithButton(Program.StaticObjects.StartButton);

            await RespondAsync(components: components.Build());
        }

        [SlashCommand("editmessage", "Edits the message of the LFG")]
        public async Task EditMessage([Summary("custom-message", "New custom message for the event")] string customMessage)
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
                await RespondAsync(text: "Only the Author of the Event can change the custom message", ephemeral: true);

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
            await RespondAsync(text: "Custom Message updated", ephemeral: true);
        }

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
                DateTimeOffset newDateTime = new(dtParsed, new TimeSpan(1, 0, 0));

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

        [SlashCommand("roll", "Rolls a number for every player of the LFG")]
        public async Task Roll()
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

            Embed originalEmbed = message.Embeds.First() as Embed;
            EmbedBuilder embed = new()
            {
                Title = "Loot Roll",
                Description = "Rolls are starting:\n\n",
                Color = Color.Gold,
            };

            int highestNumber = -1;
            string highestNumberUser = "";
            int highestNumberUserCount = 0;

            foreach (EmbedField field in originalEmbed.Fields)
            {
                if (field.Name == "Custom Message" || field.Name == "Time")
                {
                    continue;
                }

                int randomNumber = Program.Random.Next(101);
                string userName = field.Value.Split("\n")[0];
                embed.Description += $"{userName} has rolled {randomNumber}\n";

                if (randomNumber == highestNumber)
                {
                    highestNumberUserCount++;
                    highestNumberUser += $" {userName}";
                }

                if (randomNumber > highestNumber)
                {
                    highestNumberUserCount = 1;
                    highestNumber = randomNumber;
                    highestNumberUser = userName;
                }
            }

            embed.Description += $"\nThe Winner of the Rolls is {highestNumberUser} with a roll of {highestNumber}";

            while (highestNumberUserCount > 1)
            {
                embed.Description += "\n\nMultiple Users won, rerolling\n\n";

                string[] usersWon = highestNumberUser.Split(" ");

                highestNumber = -1;
                highestNumberUser = "";

                foreach (string user in usersWon)
                {
                    int randomNumber = Program.Random.Next(101);
                    embed.Description += $"{user} has rolled {randomNumber}\n";

                    if (randomNumber == highestNumber)
                    {
                        highestNumberUserCount++;
                        highestNumberUser += $" {user}";
                    }

                    if (randomNumber > highestNumber)
                    {
                        highestNumberUserCount = 1;
                        highestNumber = randomNumber;
                        highestNumberUser = user;
                    }
                }

                embed.Description += $"\nThe Winner of the Rolls is {highestNumberUser} with a roll of {highestNumber}";
            }

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("when", "Shows the time of the lfg if set")]
        public async Task When()
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

            Embed embed = message.Embeds.First() as Embed;
            EmbedField timeField = new();

            foreach (EmbedField field in embed.Fields)
            {
                if (field.Name == "Time")
                {
                    timeField = field;
                }
            }

            if (string.IsNullOrEmpty(timeField.Name))
            {
                await RespondAsync(text: "This event doesn't have a time set", ephemeral: true);

                return;
            }

            string time = timeField.Value.Split("\n")[0];
            long unixSeconds = long.Parse(time.Replace("<t:", "").Replace(":F>", ""));

            await RespondAsync($"The event starts at <t:{unixSeconds}:F>\n\nThat's <t:{unixSeconds}:R>");
        }
    }
}