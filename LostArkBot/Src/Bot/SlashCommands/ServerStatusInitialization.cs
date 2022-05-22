﻿using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class ServerStatusInitialization
    {
        public static SlashCommandBuilder ServerStatus()
        {
            SlashCommandBuilder serverStatusCommand = new SlashCommandBuilder()
                .WithName("serverstatus")
                .WithDescription("Shows the current Status of the Server Wei");

            return serverStatusCommand;
        }
    }
}