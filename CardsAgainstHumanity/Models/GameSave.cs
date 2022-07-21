using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class GameSave
    {
        public int? GameID { get; set; }
        public string GameName { get; set; }
        public string DecksIDs { get; set; }
        public string OptionsIDs { get; set; }
        public string Message { get; set; }
    }
}
