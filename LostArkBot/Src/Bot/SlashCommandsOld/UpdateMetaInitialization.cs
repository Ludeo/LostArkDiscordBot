using Discord;

namespace LostArkBot.Src.Bot.SlashCommandsOld
{
    internal class UpdateMetaInitialization
    {
        public static SlashCommandBuilder UpdateMeta()
        {
            SlashCommandBuilder updateMetaCommand = new SlashCommandBuilder()
                                                    .WithName("updatemeta")
                                                    .WithDescription("Updates your currently logged in character with information from meta-game")
                                                    .AddOption(new SlashCommandOptionBuilder()
                                                               .WithName("twitch-name")
                                                               .WithDescription("Name of your character, caps sensitive!")
                                                               .WithRequired(true)
                                                               .WithType(ApplicationCommandOptionType.String));

            return updateMetaCommand;
        }
    }
}
