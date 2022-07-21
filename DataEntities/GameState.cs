using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class GameState
    {
        //turn order, card czar, player cards, player scores, game id
        public int? GameStateID { get; set; }
        public bool ShowPlayedCards { get; set; }
        public int GameID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Player CurrentPlayerCzar 
        {
            get
            {
                return Players[CurrentOrderIndex];
            }
        }
        public int CurrentOrderIndex { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();

        public Player GetNextPlayerCzar()
        {
            if (CurrentOrderIndex >= (Players.Count - 1))
            {
                CurrentOrderIndex = 0;
            }
            else
            {
                CurrentOrderIndex++;
            }

            Players.ForEach(x => x.IsCzar = false);

            Players[CurrentOrderIndex].IsCzar = true;

            return Players[CurrentOrderIndex];
        }

        public DataEntities.BlackCard CurrentBlackCard { get; set; }

        public List<DataEntities.BlackCard> BlackCardsAlreadyPlayed { get; set; } = new List<BlackCard>();

        [Newtonsoft.Json.JsonIgnore]
        public string Data
        {
            get
            {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(this);

                return data;
            }
        }

        public GameState GameStateFromString(string data)
        {
            var ret = new GameState();

            ret = Newtonsoft.Json.JsonConvert.DeserializeObject<GameState>(data);

            this.GameStateID = ret.GameStateID;
            this.GameID = ret.GameID;
            this.CurrentOrderIndex = ret.CurrentOrderIndex;
            this.Players = ret.Players;
            this.CurrentBlackCard = ret.CurrentBlackCard;
            this.BlackCardsAlreadyPlayed = ret.BlackCardsAlreadyPlayed;

            return ret;
        }

    }


    public class Player
    {
        public bool IsHost { get; set; }
        public bool IsCzar { get; set; }
        public string GUID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int Score { get; set; }
        public List<WhiteCard> SubmittedCards { get; set; } = new List<WhiteCard>();
        public List<WhiteCard> WhiteCards { get; set; } = new List<WhiteCard>();
    }
}
