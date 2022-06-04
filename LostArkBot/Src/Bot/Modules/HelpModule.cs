using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class HelpModule
    {
        public static async Task HelpAsync(SocketSlashCommand command)
        {
            EmbedBuilder embed = new()
            {
                Title = "Help",
                Color = Color.Gold,
                Description = $"**/account**\nShows all your registered characters\n\n" +
                $"**/delete [character-name]**\nDeletes the given character\n\n" +
                $"**/editmessage [custom message]**\nEdits the Custom Message of the lfg event that belongs to the thread\n\n" +
                $"**/edittime [time]**\nEdits time of the lfg event, has to be in this format: DD/MM hh:mm\n\n" +
                $"**/help**\nShows an overview of all commands\n\n" +
                $"**/lfg (custom-message)**\nCreates an LFG Event with an optional custom message\n\n" +
                $"**/profile [character-name]**\nShows the profile of the given character\n\n" +
                $"**/profilemeta [character-name]**\nShows a picture of the meta-game profile of the given character name\n\n" +
                $"**/register [character-name] [class-name] [item-level] [engravings] [crit] [spec] [dom] [swift] [end] [exp] (custom-message) (profile-picture)**\n" +
                    $"Registers a new character with the given stats, optionally adds a custom message and/or profile picture\n\n" +
                $"**/registermeta [twitch-name]**\nRegisters the currently online character with information from meta-game with the given twitch-name\n\n" +
                $"**/roll**\nRolls a number for every player that has joined the lfg event and selects a winner based on the highest number\n\n" +
                $"**/serverstatus**\nShows the Server Status of the Wei server\n\n" +
                $"**/update [character-name] (class-name) (item-level) (engravings) (crit) (spec) (dom) (swift) (end) (exp) (custom-message) (profile-picture)**\n" +
                    $"Updates the given character with the given parameters. Everything except the character name is optional\n\n" + 
                $"**/updatemeta [twitch-name]**\nUpdates the currently online character with information from meta-game with the given twitch-name\n\n" +
                $"**/when**\nShows when the lfg starts (if set). It shows the date as well as in how many hours and minutes\n\n",
            };

            await command.RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}