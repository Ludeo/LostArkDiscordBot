using System;
using System.Collections.Generic;

namespace LostArkBot.databasemodels
{
    public partial class StaticGroup
    {
        public StaticGroup()
        {
            Characters = new HashSet<Character>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int LeaderId { get; set; }

        public virtual User Leader { get; set; }

        public virtual ICollection<Character> Characters { get; set; }
    }
}
