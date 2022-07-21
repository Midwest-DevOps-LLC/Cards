using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class GameDeck : BaseEntity
    {
        public int? GameDeckID { get; set; }
        public int GameID { get; set; }
        public int DeckID { get; set; }
    }
}
