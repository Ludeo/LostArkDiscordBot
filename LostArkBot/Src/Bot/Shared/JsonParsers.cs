using LostArkBot.Src.Bot.FileObjects;
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

        public static Task WriteCharactersAsync(List<Character> values)
        {
            return File.WriteAllTextAsync(FileConfigurations.CharactersJson, JsonSerializer.Serialize(values));
        }

        public static void WriteCharacters(List<Character> values)
        {
            WriteCharactersAsync(values).GetAwaiter().GetResult();
        }



        public static async Task<List<Dictionary<string, Engraving>>> GetEngravingsFromJsonAsync()
        {
            return JsonSerializer.Deserialize<List<Dictionary<string, Engraving>>>(await File.ReadAllTextAsync(FileConfigurations.EngravingsJson));
        }

        public static Task WriteEngravingsAsync(List<Dictionary<string, Engraving>> values)
        {
            return File.WriteAllTextAsync(FileConfigurations.EngravingsJson, JsonSerializer.Serialize(values));
        }

        public static void WriteEngravings(List<Dictionary<string, Engraving>> values)
        {
            WriteEngravingsAsync(values).GetAwaiter().GetResult();
        }



        public static async Task<List<StaticGroup>> GetStaticGroupsFromJsonAsync()
        {
            return JsonSerializer.Deserialize<List<StaticGroup>>(await File.ReadAllTextAsync(FileConfigurations.StaticGroupsJson));
        }

        public static Task WriteStaticGroupsAsync(List<StaticGroup> values)
        {
            return File.WriteAllTextAsync(FileConfigurations.StaticGroupsJson, JsonSerializer.Serialize(values));
        }

        public static void WriteStaticGroups(List<StaticGroup> values)
        {
            WriteStaticGroupsAsync(values).GetAwaiter().GetResult();
        }



        public static async Task<List<ChallengeGuardian>> GetChallengeGuardiansFromJson()
        {
            return JsonSerializer.Deserialize<List<ChallengeGuardian>>(await File.ReadAllTextAsync(FileConfigurations.ChallengeGuardiansJson));
        }

        public static Task WriteChallengeGuardiansAsync(List<ChallengeGuardian> values)
        {
            return File.WriteAllTextAsync(FileConfigurations.ChallengeGuardiansJson, JsonSerializer.Serialize(values));
        }

        public static void WriteChallengeGuardians(List<ChallengeGuardian> values)
        {
            WriteChallengeGuardiansAsync(values).GetAwaiter().GetResult();
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

        public static void WriteMerchants(List<UserSubscription> values)
        {
            File.WriteAllText(FileConfigurations.MerchantsJson, JsonSerializer.Serialize(values));
        }
    }
}
