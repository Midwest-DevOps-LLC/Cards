using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class DeckSave
    {
        public int? DeckID { get; set; }
        public string DeckName { get; set; }
        public string WhiteCardIDs { get; set; }
        public string BlackCardIDs { get; set; }
        public string Message { get; set; }
    }
}
