using System;
using System.Collections.Generic;

namespace LostArkBot.databasemodels
{
    public partial class ActiveMerchant
    {
        public string MerchantId { get; set; }
        public string Name { get; set; }
        public string Zone { get; set; }
        public int CardId { get; set; }
        public int RapportId { get; set; }
        public int Votes { get; set; }

        public virtual MerchantItem Card { get; set; }
        public virtual MerchantItem Rapport { get; set; }
    }
}
