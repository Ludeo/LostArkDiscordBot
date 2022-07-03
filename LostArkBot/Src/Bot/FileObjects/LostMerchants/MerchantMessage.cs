namespace LostArkBot.Src.Bot.FileObjects.LostMerchants
{
    public class MerchantMessage
    {
        public string MerchantId { get; set; }

        public ulong MessageId { get; set; }

        public MerchantMessage(string merchantId, ulong messageId)
        {
            MerchantId = merchantId;
            MessageId = messageId;
        }
    }
}