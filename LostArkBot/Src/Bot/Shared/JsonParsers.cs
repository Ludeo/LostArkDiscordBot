using LostArkBot.Src.Bot.FileObjects;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Shared
{
    public class JsonParsers
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
            if (!File.Exists(FileConfigurations.ChallengeNamesJson))
            {
                await File.WriteAllTextAsync(FileConfigurations.ChallengeNamesJson, JsonSerializer.Serialize(new ChallengeNames()));
            }
        }

        public static async Task<ChallengeNames> GetChallengeNamesFromJson()
        {
            return JsonSerializer.Deserialize<ChallengeNames>(await File.ReadAllTextAsync(FileConfigurations.ChallengeNamesJson));
        }

        public static async Task WriteChallengeNamesAsync(ChallengeNames values)
        {
            await File.WriteAllTextAsync(FileConfigurations.ChallengeNamesJson, JsonSerializer.Serialize(values));
        }
    }
}