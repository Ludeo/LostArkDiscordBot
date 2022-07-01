using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
