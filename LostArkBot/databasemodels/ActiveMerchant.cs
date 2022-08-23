namespace LostArkBot.databasemodels;

// ReSharper disable once ClassNeverInstantiated.Global
public class ActiveMerchant
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Id { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Name { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Zone { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int CardId { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int RapportId { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Votes { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public virtual MerchantItem Card { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public virtual MerchantItem Rapport { get; set; }
}