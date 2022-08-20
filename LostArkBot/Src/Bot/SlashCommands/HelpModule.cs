using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class HelpModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("help", "Displays help for all the commands")]
        public async Task Help([Summary("command-name", "Name of the command you want help for")] Commands commandSpecific = Commands.Default)
        {
            await DeferAsync(ephemeral: true);

#if DEBUG
            IReadOnlyCollection<SocketApplicationCommand> commandCollection = await Context.Guild.GetApplicationCommandsAsync();
#else
            IReadOnlyCollection<SocketApplicationCommand> commandCollection = await Context.Client.GetGlobalApplicationCommandsAsync();
#endif
            
            EmbedBuilder embed = new()
            {
                Title = "Help",
                Description = "List of all commands:",
                Color = Color.Gold,
            };

            if (commandSpecific == Commands.Default)
            {
                List<SocketApplicationCommand> commandsUnsorted = new(commandCollection);
                IOrderedEnumerable<SocketApplicationCommand> commands = commandsUnsorted.OrderBy(x => x.Name);
                IEnumerable<string> commandValues = Enum.GetValues(typeof(Commands)).OfType<Commands>().Select(x => x.ToString().ToLower());

                foreach (SocketApplicationCommand command in commands)
                {

                    if (command.Type == ApplicationCommandType.Slash)
                    {
                        string subcommands = "";
                        foreach (SocketApplicationCommandOption option in command.Options)
                        {
                            subcommands += $"\uFEFF \uFEFF \uFEFF \uFEFF \uFEFF \uFEFF \uFEFF **{option.Name}** - {option.Description}\n";
                            //embed.AddField($"\t - {option.Name} - {option.Description}", false);
                        }
                        if (subcommands == "")
                        {
                            subcommands = "*No sub-commands*";
                        }
                        if (commandValues.Contains(command.Name.ToLower().Replace("-", "")))
                        {
                            embed.AddField($"/{command.Name} - {command.Description}", subcommands, false);
                        }
                    }
                }
            }
            else
            {
                SocketApplicationCommand command = commandCollection.First(x => x.Name.Replace("-", "").ToLower() == commandSpecific.ToString().ToLower());

                embed.Title += " for " + command.Name;
                embed.Description = command.Description;

                foreach (SocketApplicationCommandOption option in command.Options)
                {
                    embed.AddField(option.Name, option.Description);
                }
            }

            await FollowupAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}