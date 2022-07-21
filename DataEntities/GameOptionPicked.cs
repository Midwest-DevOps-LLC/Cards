using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class GameOptionPicked
    {
        public int? GameOptionPickedID { get; set; }
        public int GameID { get; set; }
        public int GameOptionID { get; set; }
        public bool Active { get; set; }
    }
}
