using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class Game : BaseEntity
    {
        public int? GameID { get; set; }

        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public List<Deck> Decks { get; set; } = new List<Deck>(); //Add options to game view screen for blank cards and other stuff. Add join game on viewing on of a game on view. Create game_user on join. Send to game screen with all signalr javascript. This is where the game will be played
        public List<User> Users { get; set; } = new List<User>();
        public List<GameOption> GameOptions { get; set; } = new List<GameOption>();
    }
}
