using Discord;

namespace LostArkBot.Src.Bot.SlashCommandsOld
{
    internal class RegisterMetaInitialization
    {
        public static SlashCommandBuilder RegisterMeta()
        {
            SlashCommandBuilder registerMetaCommand = new SlashCommandBuilder()
                                                    .WithName("registermeta")
                                                    .WithDescription("Registers your currently logged in character with information from meta-game")
                                                    .AddOption(new SlashCommandOptionBuilder()
                                                               .WithName("twitch-name")
                                                               .WithDescription("Name of your character, caps sensitive!")
                                                               .WithRequired(true)
                                                               .WithType(ApplicationCommandOptionType.String));

            return registerMetaCommand;
        }
    }
}
