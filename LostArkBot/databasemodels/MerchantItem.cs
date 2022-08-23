using System.Collections.Generic;

namespace LostArkBot.databasemodels;

// ReSharper disable once ClassNeverInstantiated.Global
public class MerchantItem
{
    public MerchantItem()
    {
        this.ActiveMerchantCards = new HashSet<ActiveMerchant>();
        this.ActiveMerchantRapports = new HashSet<ActiveMerchant>();
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Id { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Name { get; init; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Rarity { get; init; }

    public virtual ICollection<ActiveMerchant> ActiveMerchantCards { get; }

    public virtual ICollection<ActiveMerchant> ActiveMerchantRapports { get; }
}