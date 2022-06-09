using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class AddUserModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
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

            if(originalEmbed.Fields.Any(x => x.Value.Contains(user.Mention)))
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

            foreach(EmbedField field in originalEmbed.Fields)
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
    }
}