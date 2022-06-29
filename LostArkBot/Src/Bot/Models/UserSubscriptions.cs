using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Models
{
    internal class UserSubscriptions
    {
        public ulong UserId { get; set; }

        public List<int> SubscribedItems { get; set; }

        public UserSubscriptions(ulong userId, List<int> subscribedItems)
        {
            UserId = userId;
            SubscribedItems = subscribedItems;
        }
    }
}
