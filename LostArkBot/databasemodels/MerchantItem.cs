using System;
using System.Collections.Generic;

namespace LostArkBot.databasemodels
{
    public partial class MerchantItem
    {
        public MerchantItem()
        {
            ActiveMerchantCards = new HashSet<ActiveMerchant>();
            ActiveMerchantRapports = new HashSet<ActiveMerchant>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Rarity { get; set; }

        public virtual ICollection<ActiveMerchant> ActiveMerchantCards { get; set; }
        public virtual ICollection<ActiveMerchant> ActiveMerchantRapports { get; set; }
    }
}
