using System;
using System.Collections.Generic;

namespace LostArkBot.databasemodels
{
    public partial class ChallengeGuardian
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WeekNumber { get; set; }
    }
}
