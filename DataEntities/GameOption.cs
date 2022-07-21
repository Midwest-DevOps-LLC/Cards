using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class GameOption
    {
        public int? GameOptionID { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}
