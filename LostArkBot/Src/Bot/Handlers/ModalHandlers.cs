using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Shared;
using LostArkBot.Src.Bot.SlashCommands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    public class ModalHandlers
    {
        public static async Task ModalHandler(SocketModal modal)
        {
            await modal.DeferAsync();

            if (modal.Data.CustomId[..3] == "eng")
            {
                IEnumerable<string> nonEmptyEngravings = modal.Data.Components.ToList().FindAll(x => x.Value.Trim() != "").Select(x => x.Value.ToTitleCase());
                string engravingsString = string.Join(",", nonEmptyEngravings);
                List<Character> chars = await JsonParsers.GetCharactersFromJsonAsync();
                Character character = chars.Find(x => x.CharacterName.ToLower() == modal.Data.CustomId[4..].Trim().ToLower());

                if (character == null)
                {
                    IMessage message = await modal.FollowupAsync("auto-delete");
                    await message.DeleteAsync();
                    await modal.FollowupAsync($"No character named {character.CharacterName} was found", ephemeral: true);
                    return;
                }

                character.Engravings = engravingsString;
                await JsonParsers.WriteCharactersAsync(chars);
                SocketGuildUser user = await modal.Channel.GetUserAsync(modal.User.Id) as SocketGuildUser;
                EmbedBuilder embedBuilder = await new CharactersModule().CreateEmbedAsync(character.CharacterName, (character) =>
                {
                    return user;
                });

                await modal.FollowupAsync(embed: embedBuilder.Build());
            }
        }
    }
}