using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class GameIndex
    {
        public List<DataEntities.Game> PublicGames { get; set; } = new List<DataEntities.Game>();
        public List<DataEntities.Game> YourGames { get; set; } = new List<DataEntities.Game>();
    }
}
