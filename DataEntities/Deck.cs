using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class Deck
    {
        public int? DeckID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }

        public List<WhiteCard> WhiteCards { get; set; } = new List<WhiteCard>();
        public List<BlackCard> BlackCards { get; set; } = new List<BlackCard>();
    }
}
