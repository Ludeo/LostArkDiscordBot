using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class HelpModule
    {
        public static async Task HelpAsync(SocketSlashCommand command)
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Help",
                Color = Color.Gold,
                Description = $"**/account**\nShows all your registered characters\n\n" +
                $"**/delete [character-name]**\nDeletes the given character\n\n" +
                $"**/editmessage [message-id] [custom message]**\nEdits the Custom Message of the given Message\n\n" +
                $"**/help**\nShows an overview of all commands\n\n" +
                $"**/lfg (custom-message)**\nCreates an LFG Event with an optional custom message\n\n" +
                $"**/profile [character-name]**\nShows the profile of the given character\n\n" +
                $"**/register [character-name] [class-name] [item-level] [engravings] [crit] [spec] [dom] [swift] [end] [exp] (custom-message) (profile-picture)**\n" +
                    $"Registers a new character with the given stats, optionally adds a custom message and/or profile picture\n\n" +
                $"**/roll [message-id]**\nRolls a number for every player that has joined the lfg event and selects a winner based on the highest number\n\n" +
                $"**/serverstatus**\nShows the Server Status of the Wei server\n\n" +
                $"**/update [character-name] (class-name) (item-level) (engravings) (crit) (spec) (dom) (swift) (end) (exp) (custom-message) (profile-picture)**\n" +
                    $"Updates the given character with the given parameters. Everything except the character name is optional\n\n",
            };

            await command.RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}