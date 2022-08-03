using Discord;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.FileObjects.LostMerchants;
using LostArkBot.Src.Bot.FileObjects.MetaGame;
using LostArkBot.Src.Bot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Shared
{
    internal class JsonParsers
    {

        public static Config GetConfigFromJson()
        {
            if (!File.Exists(FileConfigurations.ConfigJson))
            {
                Config configFile = new();
                File.WriteAllText(FileConfigurations.ConfigJson, JsonSerializer.Serialize(configFile));

                return configFile;
            }

            return JsonSerializer.Deserialize<Config>(File.ReadAllText(FileConfigurations.ConfigJson));
        }

        public static async Task WriteConfigAsync(Config values)
        {
            await File.WriteAllTextAsync(FileConfigurations.ConfigJson, JsonSerializer.Serialize(values));
        }

        public static async Task InitializeAllFiles()
        {
            if (!File.Exists(FileConfigurations.CharactersJson))
            {
                await File.WriteAllTextAsync(FileConfigurations.CharactersJson, JsonSerializer.Serialize(new List<Character>()));
            }

            if (!File.Exists(FileConfigurations.EngravingsJson))
            {
                await File.WriteAllTextAsync(FileConfigurations.EngravingsJson, JsonSerializer.Serialize(new List<Dictionary<string, Engraving>>()));
            }

            if (!File.Exists(FileConfigurations.StaticGroupsJson))
            {
                await File.WriteAllTextAsync(FileConfigurations.StaticGroupsJson, JsonSerializer.Serialize(new List<StaticGroup>()));
            }

            if (!File.Exists(FileConfigurations.ChallengeNamesJson))
            {
                await File.WriteAllTextAsync(FileConfigurations.ChallengeNamesJson, JsonSerializer.Serialize(new ChallengeNames()));
            }

            if (!File.Exists(FileConfigurations.SubscriptionsJson))
            {
                await File.WriteAllTextAsync(FileConfigurations.SubscriptionsJson, JsonSerializer.Serialize(new List<UserSubscription>()));
            }

            if (!File.Exists(FileConfigurations.ActiveMerchantsJson))
            {
                await File.WriteAllTextAsync(FileConfigurations.ActiveMerchantsJson, JsonSerializer.Serialize(new List<Merchant>()));
            }
        }

        public static async Task<List<Character>> GetCharactersFromJsonAsync()
        {
            return JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync(FileConfigurations.CharactersJson));
        }

        public static async Task WriteCharactersAsync(List<Character> values)
        {
            await File.WriteAllTextAsync(FileConfigurations.CharactersJson, JsonSerializer.Serialize(values));
        }


        public static async Task<List<Dictionary<string, Engraving>>> GetEngravingsFromJsonAsync()
        {
            return JsonSerializer.Deserialize<List<Dictionary<string, Engraving>>>(await File.ReadAllTextAsync(FileConfigurations.EngravingsJson));
        }

        public static async Task WriteEngravingsAsync(List<Dictionary<string, Engraving>> values)
        {
            await File.WriteAllTextAsync(FileConfigurations.EngravingsJson, JsonSerializer.Serialize(values));
        }


        public static async Task<List<StaticGroup>> GetStaticGroupsFromJsonAsync()
        {
            return JsonSerializer.Deserialize<List<StaticGroup>>(await File.ReadAllTextAsync(FileConfigurations.StaticGroupsJson));
        }

        public static async Task WriteStaticGroupsAsync(List<StaticGroup> values)
        {
            await File.WriteAllTextAsync(FileConfigurations.StaticGroupsJson, JsonSerializer.Serialize(values));
        }


        public static async Task<ChallengeNames> GetChallengeNamesFromJson()
        {
            return JsonSerializer.Deserialize<ChallengeNames>(await File.ReadAllTextAsync(FileConfigurations.ChallengeNamesJson));
        }

        public static async Task WriteChallengeNamesAsync(ChallengeNames values)
        {
            await File.WriteAllTextAsync(FileConfigurations.ChallengeNamesJson, JsonSerializer.Serialize(values));
        }


        public static async Task<List<UserSubscription>> GetMerchantSubsFromJsonAsync()
        {
            return JsonSerializer.Deserialize<List<UserSubscription>>(await File.ReadAllTextAsync(FileConfigurations.SubscriptionsJson));
        }

        public static async Task WriteMerchantsAsync(List<UserSubscription> values)
        {
            await File.WriteAllTextAsync(FileConfigurations.SubscriptionsJson, JsonSerializer.Serialize(values));
        }


        public static async Task<List<Merchant>> GetActiveMerchantsJsonAsync()
        {
            return JsonSerializer.Deserialize<List<Merchant>>(await File.ReadAllTextAsync(FileConfigurations.ActiveMerchantsJson));
        }

        public static async Task WriteActiveMerchantsAsync(List<Merchant> values)
        {
            await File.WriteAllTextAsync(FileConfigurations.ActiveMerchantsJson, JsonSerializer.Serialize(values));
        }
    }
}
