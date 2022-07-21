using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class GamePlay
    {
        public DataEntities.Game Game { get; set; }
        public DataEntities.GameState GameState { get; set; }
    }
}
