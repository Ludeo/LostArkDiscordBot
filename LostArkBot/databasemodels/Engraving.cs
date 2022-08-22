using System;
using System.Collections.Generic;

namespace LostArkBot.databasemodels
{
    public partial class Engraving
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc0 { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
        public bool Penalty { get; set; }
    }
}
