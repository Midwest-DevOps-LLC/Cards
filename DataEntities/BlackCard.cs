using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class BlackCard
    {
        public int? BlackCardID { get; set; }
        public string Text { get; set; }
        public string DeckName { get; set; }
        public int Draw { get; set; }
        public int Pick { get; set; }

        public int CreatedBy { get; set; }

        public List<DataEntities.Deck> Decks { get; set; } = new List<Deck>();
    }
}
