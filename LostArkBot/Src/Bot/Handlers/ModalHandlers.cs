using Discord;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using LostArkBot.Src.Bot.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    public class ModalHandlers
    {
        private readonly LostArkBotContext dbcontext;

        public ModalHandlers(LostArkBotContext context)
        {
            dbcontext = context;    
        }

        public async Task ModalHandler(SocketModal modal)
        {
            await modal.DeferAsync();

            if (modal.Data.CustomId[..3] == "eng")
            {
                IEnumerable<string> nonEmptyEngravings = modal.Data.Components.ToList().FindAll(x => x.Value.Trim() != "").Select(x => x.Value.ToTitleCase());
                string engravingsString = string.Join(",", nonEmptyEngravings);
                string characterName = modal.Data.CustomId[4..];
                Character character = dbcontext.Characters.Where(x => x.CharacterName.ToLower() == characterName.Trim().ToLower()).FirstOrDefault();

                if (character == null)
                {
                    IMessage message = await modal.FollowupAsync("auto-delete");
                    await message.DeleteAsync();
                    await modal.FollowupAsync($"No character named {character.CharacterName} was found", ephemeral: true);
                    return;
                }

                character.Engravings = engravingsString;
                dbcontext.Characters.Update(character);
                await dbcontext.SaveChangesAsync();

                SocketGuildUser user = await modal.Channel.GetUserAsync(modal.User.Id) as SocketGuildUser;
                EmbedBuilder embedBuilder = Utils.CreateProfileEmbed(character.CharacterName, dbcontext, (character) =>
                {
                    return user;
                });

                if(embedBuilder is null)
                {
                    IMessage deleteMessage = await modal.FollowupAsync("auto-delete");
                    await deleteMessage.DeleteAsync();
                    await modal.FollowupAsync(text: $"{character.CharacterName} doesn't exist. You can add it with **/characters add**", ephemeral: true);
                    return;
                }

                await modal.FollowupAsync(embed: embedBuilder.Build());
            }
        }
    }
}