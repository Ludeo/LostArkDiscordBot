namespace LostArkBot.Bot.FileObjects.LostMerchants;

public class MerchantMessage
{
    public MerchantMessage(string merchantId, ulong? messageId, bool isDeleted = false)
    {
        this.MerchantId = merchantId;
        this.MessageId = messageId;
        this.IsDeleted = isDeleted;
    }

    public string MerchantId { get; }

    public ulong? MessageId { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool IsDeleted { get; set; }
}