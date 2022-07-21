using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class GameView
    {
        public DataEntities.Game Game { get; set; }
        public List<DataEntities.GameOption> GameOptionsAvailable { get; set; } = new List<DataEntities.GameOption>();

        public GameView()
        {

        }

        public GameView(DataEntities.Game e)
        {
            this.Game = e;
        }
    }
}
