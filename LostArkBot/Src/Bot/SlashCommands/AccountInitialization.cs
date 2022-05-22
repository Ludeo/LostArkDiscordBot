using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class AccountInitialization
    {
        public static SlashCommandBuilder Account()
        {
            SlashCommandBuilder accountCommand = new SlashCommandBuilder()
                .WithName("account")
                .WithDescription("Shows all your registered characters as well as your default character");

            return accountCommand;
        }
    }
}