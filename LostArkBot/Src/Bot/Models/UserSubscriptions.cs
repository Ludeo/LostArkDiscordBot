using System.Collections.Generic;

namespace LostArkBot.Src.Bot.Models
{
    public class UserSubscription
    {
        public ulong UserId { get; set; }

        public List<int> SubscribedItems { get; set; }

        public UserSubscription(ulong userId, List<int> subscribedItems)
        {
            UserId = userId;
            SubscribedItems = subscribedItems;
        }
    }
}