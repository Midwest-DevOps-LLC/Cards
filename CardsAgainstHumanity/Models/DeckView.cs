using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class DeckView
    {
        public DataEntities.Deck Deck { get; set; }

        public DeckView()
        {

        }

        public DeckView(DataEntities.Deck e)
        {
            this.Deck = e;
        }
    }
}
