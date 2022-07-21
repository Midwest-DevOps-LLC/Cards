using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class DeckIndex
    {
        public List<DataEntities.Deck> Decks { get; set; } = new List<DataEntities.Deck>();
        public List<DataEntities.Deck> CustomDecks { get; set; } = new List<DataEntities.Deck>();
    }
}
