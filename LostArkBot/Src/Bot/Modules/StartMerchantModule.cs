using System;
using System.IO;
using System.Text.Json;
using System.Timers;
using Discord;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;

namespace LostArkBot.Bot.Modules
{
    /// <summary>
    ///     Module for the startmerchant command.
    /// </summary>
    public static class StartMerchantModule
    {
        public static void StartMerchantAsync()
        {
            Config config = Config.Default;
            Timer timer = Program.Timer;

            if (timer.Enabled)
            {
                return;
            }

            timer.Interval = 60000;
            timer.Enabled = true;
            timer.Elapsed += OnTimedEvent;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            DateTime time = DateTime.Now;
            DiscordSocketClient client = Program.Client;
            Config config = Config.Default;

            long unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            long merchant1 = config.Merchant1;
            long merchant2 = config.Merchant2;
            long merchant3 = config.Merchant3;

            if (time.Hour is 1 or 4 or 5 or 7 or 8 or 11 or 13 or 16 or 17 or 19 or 20 or 23 && time.Minute is > 30 and < 55)
            {
                // checks if the difference between unixTime and merchant1 is bigger than 30 minutes.
                if (unixTime - merchant1 > 1800)
                {
                    EmbedBuilder embedBuilder = new()
                    {
                        Title = "Following Merchants are available till " + time.Hour + ":55 (Server time)",
                        Description = "Lucas in Yudia\nMorris in East Luterra\nMac in Anikka\nJeffrey in Shushire\nDorella in Feiton",
                    };

                    client.GetGuild(config.Server).GetTextChannel(config.MerchantChannel)
                          .SendMessageAsync(string.Empty, false, embedBuilder.Build());

                    config.Merchant1 = unixTime;
                    string jsonData = JsonSerializer.Serialize(config);
                    File.WriteAllText("config.json", jsonData);
                }
            }

            if (time.Hour is 0 or 2 or 5 or 6 or 8 or 9 or 12 or 14 or 17 or 18 or 20 or 21 && time.Minute is > 30 and < 55)
            {
                // checks if the difference between unixTime and merchant2 is bigger than 30 minutes.
                if (unixTime - merchant2 > 1800)
                {
                    EmbedBuilder embedBuilder = new()
                    {
                        Title = "Following Merchants are available till " + time.Hour + ":55 (Server time)",
                        Description = "Malone in West Luterra\nBurt in East Luterra\nOliver in Tortoyk\nNox in Arthetine\nAricer in Rohendel\n"
                                    + "Rayni in Punika",
                    };

                    client.GetGuild(config.Server).GetTextChannel(config.MerchantChannel)
                          .SendMessageAsync(string.Empty, false, embedBuilder.Build());

                    config.Merchant2 = unixTime;
                    string jsonData = JsonSerializer.Serialize(config);
                    File.WriteAllText("config.json", jsonData);
                }
            }

            if (time.Hour is 0 or 3 or 4 or 6 or 7 or 10 or 12 or 15 or 16 or 18 or 19 or 22 && time.Minute is > 30 and < 55)
            {
                // checks if the difference between unixTime and merchant3 is bigger than 30 minutes.
                if (unixTime - merchant3 > 1800)
                {
                    EmbedBuilder embedBuilder = new()
                    {
                        Title = "Following Merchants are available till " + time.Hour + ":55 (Server time)",
                        Description = "Ben in Rethramis\nPeter in North Vern\nLaitir in Yorn\nEvan in South Vern",
                    };

                    client.GetGuild(config.Server).GetTextChannel(config.MerchantChannel)
                          .SendMessageAsync(string.Empty, false, embedBuilder.Build());

                    config.Merchant3 = unixTime;
                    string jsonData = JsonSerializer.Serialize(config);
                    File.WriteAllText("config.json", jsonData);
                }
            }
        }
    }
}