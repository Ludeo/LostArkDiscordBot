using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.FileObjects.LostMerchants;
using System.Collections.Generic;
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

        public static async Task<List<WebsiteMerchant>> GetActiveMerchantsJsonAsync()
        {
            return JsonSerializer.Deserialize<List<WebsiteMerchant>>(await File.ReadAllTextAsync(FileConfigurations.ActiveMerchantsJson));
        }

        public static async Task WriteActiveMerchantsAsync(List<WebsiteMerchant> values)
        {
            await File.WriteAllTextAsync(FileConfigurations.ActiveMerchantsJson, JsonSerializer.Serialize(values));
        }
    }
}