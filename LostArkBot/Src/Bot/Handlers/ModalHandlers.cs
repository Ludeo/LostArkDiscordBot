using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    public class ModalHandlers
    {
        public static async Task ModalHandler(SocketModal modal)
        {
            if (modal.Data.CustomId[..3] == "eng")
            {
                IEnumerable<string> nonEmptyEngravings = modal.Data.Components.ToList<SocketMessageComponentData>().FindAll(x => x.Value.Trim() != "").Select(x => x.Value.ToTitleCase());
                string engravingsString = string.Join(",", nonEmptyEngravings);
                List<Character> chars = await JsonParsers.GetCharactersFromJsonAsync();
                Character character = chars.Find(x => x.CharacterName.ToLower() == modal.Data.CustomId[4..].Trim().ToLower());

                if (character == null)
                {
                    await modal.RespondAsync($"No character named {character.CharacterName} was found", ephemeral: true);
                    return;
                }

                character.Engravings = engravingsString;
                await JsonParsers.WriteCharactersAsync(chars);
                await modal.RespondAsync("Character updated", ephemeral: true);
            }
        }
    }
}
