using LostArkBot.databasemodels;
using LostArkBot.Src.Bot.Models.Enums;
using System;
using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects.LostMerchants
{
    public class WebsiteMerchant
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("zone")]
        public string Zone { get; set; }

        [JsonPropertyName("card")]
        public MerchantItem Card { get; set; }

        [JsonPropertyName("rapport")]
        public MerchantItem Rapport { get; set; }

        [JsonPropertyName("votes")]
        public int Votes { get; set; }

        public ActiveMerchant CastToActiveMerchant()
        {
            ActiveMerchant activeMerchant = new();
            activeMerchant.Id = Id;
            activeMerchant.Name = Name;
            activeMerchant.Zone = Zone;
            activeMerchant.Votes = Votes;
            //activeMerchant.CardId = ParseItem(Card);
            //activeMerchant.RapportId = ParseItem(Card);
            activeMerchant.Card = new databasemodels.MerchantItem
            {
                Name = Card.Name,
                Rarity = Card.Rarity.ToString(),
            };
            activeMerchant.Rapport = new databasemodels.MerchantItem
            {
                Name = Rapport.Name,
                Rarity = Rapport.Rarity.ToString(),
            };


            return activeMerchant;
        }

        private int ParseItem(MerchantItem item)
        {
            if(Enum.TryParse(item.Name, out WanderingMerchantItemsEnum result))
            {
                return (int)result;
            }

            return -1;
        }
    }
}