using System;
using System.Collections.Generic;

namespace LostArkBot.databasemodels
{
    public partial class Subscription
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }

        public virtual User User { get; set; }
    }
}
