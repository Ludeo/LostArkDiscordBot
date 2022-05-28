﻿using Discord;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class RollModule
    {
        public static async Task RollAsync(SocketSlashCommand command)
        {
            List<ThreadLinkedMessage> threadLinkedMessageList = JsonSerializer.Deserialize<List<ThreadLinkedMessage>>(File.ReadAllText("ThreadMessageLink.json"));
            ThreadLinkedMessage linkedMessage = threadLinkedMessageList.First(x => x.ThreadId == command.Channel.Id);
            ulong messageId = linkedMessage.MessageId;

            ITextChannel channel = Program.Client.GetChannel(Config.Default.LfgChannel) as ITextChannel;
            IMessage messageRaw = await channel.GetMessageAsync(messageId);
            IUserMessage message = messageRaw as IUserMessage;

            Embed originalEmbed = message.Embeds.First() as Embed;
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Loot Roll",
                Description = "Rolls are starting:\n\n",
                Color = Color.Gold,
            };

            int highestNumber = -1;
            string highestNumberUser = "";
            int highestNumberUserCount = 0;

            foreach (EmbedField field in originalEmbed.Fields)
            {
                if (field.Name == "Custom Message")
                {
                    continue;
                }

                int randomNumber = Program.random.Next(101);
                string userName = field.Value.Split("\n")[0];
                embed.Description += $"{userName} has rolled {randomNumber}\n";

                if (randomNumber == highestNumber)
                {
                    highestNumberUserCount++;
                    highestNumberUser += $" {userName}";
                }

                if (randomNumber > highestNumber)
                {
                    highestNumberUserCount = 1;
                    highestNumber = randomNumber;
                    highestNumberUser = userName;
                }
            }

            embed.Description += $"\nThe Winner of the Rolls is {highestNumberUser} with a roll of {highestNumber}";

            while (highestNumberUserCount > 1)
            {
                embed.Description += "\n\nMultiple Users won, rerolling\n\n";

                string[] usersWon = highestNumberUser.Split(" ");

                highestNumber = -1;
                highestNumberUser = "";

                foreach (string user in usersWon)
                {
                    int randomNumber = Program.random.Next(101);
                    embed.Description += $"{user} has rolled {randomNumber}\n";

                    if (randomNumber == highestNumber)
                    {
                        highestNumberUserCount++;
                        highestNumberUser += $" {user}";
                    }

                    if (randomNumber > highestNumber)
                    {
                        highestNumberUserCount = 1;
                        highestNumber = randomNumber;
                        highestNumberUser = user;
                    }
                }

                embed.Description += $"\nThe Winner of the Rolls is {highestNumberUser} with a roll of {highestNumber}";
            }

            await command.RespondAsync(embed: embed.Build());
        }
    }
}