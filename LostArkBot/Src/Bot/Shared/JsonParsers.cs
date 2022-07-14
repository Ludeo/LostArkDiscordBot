using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.FileObjects.LostMerchants;
using LostArkBot.Src.Bot.FileObjects.MetaGame;
using LostArkBot.Src.Bot.Models;
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


        public static async Task<List<ChallengeGuardian>> GetChallengeGuardiansFromJson()
        {
            return JsonSerializer.Deserialize<List<ChallengeGuardian>>(await File.ReadAllTextAsync(FileConfigurations.ChallengeGuardiansJson));
        }

        public static async Task WriteChallengeGuardiansAsync(List<ChallengeGuardian> values)
        {
            await File.WriteAllTextAsync(FileConfigurations.ChallengeGuardiansJson, JsonSerializer.Serialize(values));
        }

        public static async Task<List<UserSubscription>> GetMerchantSubsFromJsonAsync()
        {
            string json;
            try
            {
                json = await File.ReadAllTextAsync(FileConfigurations.MerchantsJson);
            }
            catch (FileNotFoundException)
            {
                json = "[]";
                await File.WriteAllTextAsync(FileConfigurations.MerchantsJson, json);
            }
            return JsonSerializer.Deserialize<List<UserSubscription>>(json);
        }

        public static async Task WriteMerchantsAsync(List<UserSubscription> values)
        {
            await File.WriteAllTextAsync(FileConfigurations.MerchantsJson, JsonSerializer.Serialize(values));
        }



        public static async Task<List<MerchantVote>> GetActiveMerchantVotesJsonAsync()
        {
            var json = JsonSerializer.Deserialize<List<MerchantVote>>(await File.ReadAllTextAsync(FileConfigurations.ActiveMerchantVotesJson));
            if (json == null)
            {
                json = new List<MerchantVote>();
            }
            return json;
        }

        public static async Task WriteActiveMerchantVotesAsync(List<MerchantVote> values)
        {
            await File.WriteAllTextAsync(FileConfigurations.ActiveMerchantVotesJson, JsonSerializer.Serialize(values));
        }

    }
}
