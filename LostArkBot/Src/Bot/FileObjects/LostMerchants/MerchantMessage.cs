namespace LostArkBot.Src.Bot.FileObjects.LostMerchants
{
    public class MerchantMessage
    {
        public string MerchantId { get; set; }

        public ulong? MessageId { get; set; }

        public bool IsDeleted { get; set; }


        public MerchantMessage(string merchantId, ulong? messageId, bool isDeleted = false)
        {
            MerchantId = merchantId;
            MessageId = messageId;
            IsDeleted = isDeleted;
        }
    }
}
