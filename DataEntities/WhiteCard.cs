using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class WhiteCard
    {
        public int? WhiteCardID { get; set; }
        public string Text { get; set; }
        public string DeckName { get; set; }

        public int CreatedBy { get; set; }

        public List<DataEntities.Deck> Decks { get; set; } = new List<Deck>();
    }
}
