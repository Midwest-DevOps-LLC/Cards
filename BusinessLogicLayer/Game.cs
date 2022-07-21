using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogicLayer
{
    public class Game : BLLManager, IDisposable
    {
        public Game(string connectionString, ref DataEntities.ValidationEvent validationEvent)
        {
            this.ConnectionString = connectionString;
            this.validationEvent = validationEvent;
        }

        public Game(MySqlConnection sqlConnection, ref DataEntities.ValidationEvent validationEvent)
        {
            this.sqlConnection = sqlConnection;
            this.validationEvent = validationEvent;
        }

        const int WhiteCardsMax = 10;

        public DataEntities.GameState CreateGame (DataEntities.Game game, DataEntities.Player host)
        {
            var ret = new DataEntities.GameState();

            ret.GameID = game.GameID.GetValueOrDefault();
            ret.CurrentOrderIndex = 0;

            var tempEvent = this.validationEvent;
            var userBAL = new BusinessLogicLayer.Users(this.GetConnection(), ref tempEvent);

            var userCheck = userBAL.GetUserByUserID(host.UserID, true);

            var userName = "";

            if (userCheck != null)
            {
                userName = userCheck.Username;
            }

            host.UserName = userName;
            host.IsHost = true;

            host.WhiteCards = GetMaxPlayerCards(host, game);

            ret.CurrentBlackCard = GetRandomBlackCard(ret, game);

            //Testing

            //host.SubmittedCards.Add(host.WhiteCards[0]);

            //End Testing

            ret.Players.Add(host);

            var s = SaveGameState(ret);

            ret.GameStateID = s;

            return ret;
        }

        public DataEntities.GameState LoadGameState (DataEntities.Game game)
        {
            var ret = new DataEntities.GameState();

            var temp = this.validationEvent;
            var gameBAL = new BusinessLogicLayer.Game(this.GetConnection(), ref temp);

            ret = gameBAL.GetLatestGameStateByGameID(game.GameID.GetValueOrDefault());

            return ret;
        }

        public DataEntities.GameState SelectCardWinner (DataEntities.Player wonPlayer, DataEntities.GameState gameState, DataEntities.Game game)
        {
            wonPlayer.Score++;
            gameState.GetNextPlayerCzar();
            gameState.CurrentBlackCard = GetRandomBlackCard(gameState, game);
            gameState.ShowPlayedCards = false;
            
            foreach (var player in gameState.Players)
            {
                var subCardsCount = player.SubmittedCards.Count();

                foreach (var submittedCards in player.SubmittedCards)
                {
                    player.WhiteCards.RemoveAll(x => x.WhiteCardID == submittedCards.WhiteCardID);
                }

                player.WhiteCards.AddRange(GetMaxPlayerCards(player, game));

                //for (int i = 0; i <= subCardsCount; i++)
                //{
                //    player.WhiteCards.Add()
                //}

                player.SubmittedCards.Clear();
            }

            var s = SaveGameState(gameState);

            //gameState.GameStateID = s;

            return gameState;
        }

        public DataEntities.GameState SubmitCard(DataEntities.Player player, DataEntities.WhiteCard whiteCard, DataEntities.GameState gameState)
        {
            //update game state and player obj that updates the selected cards in the object

            var getPlayer = gameState.Players.Where(x => x.UserID == player.UserID).ToList().FirstOrDefault();

            getPlayer.SubmittedCards = new List<DataEntities.WhiteCard>();
            getPlayer.SubmittedCards.Add(whiteCard);

            var s = SaveGameState(gameState);

            return gameState;
        }

        public DataEntities.GameState AddPlayer(DataEntities.GameState gameState, DataEntities.Player newPlayer, DataEntities.Game game)
        {
            if (gameState.Players.Select(x => x.UserID).Contains(newPlayer.UserID))
            {
                //Player already in game
            }
            else
            {
                var tempEvent = this.validationEvent;
                var userBAL = new BusinessLogicLayer.Users(this.GetConnection(), ref tempEvent);

                var userCheck = userBAL.GetUserByUserID(newPlayer.UserID, true);

                var userName = "";

                if (userCheck != null)
                {
                    userName = userCheck.Username;
                }

                newPlayer.UserName = userName;

                newPlayer.WhiteCards = GetMaxPlayerCards(newPlayer, game);

                gameState.Players.Add(newPlayer);

                var s = SaveGameState(gameState);

                return gameState;
            }

            return gameState;
        }

        public DataEntities.BlackCard GetRandomBlackCard(DataEntities.GameState gameState, DataEntities.Game game)
        {
            var ret = new DataEntities.BlackCard();

            var cardsBLL = new BusinessLogicLayer.BlackCard(this.GetConnection());

            var currentCardIDs = gameState.BlackCardsAlreadyPlayed.Select(x => x.BlackCardID.GetValueOrDefault()).ToList();
            var selectedDeckIDs = game.Decks.Select(x => x.DeckID.GetValueOrDefault()).ToList();

            var onlyPick1Cards = false;

            if (game.GameOptions.Any(x => x.GameOptionID == 1)) //Only Pick 1 cards
            {
                onlyPick1Cards = true;
            }

            ret = cardsBLL.GetRandomBlackCardBasedOnDecks(currentCardIDs, selectedDeckIDs, onlyPick1Cards);

            return ret;
        }

        public List<DataEntities.WhiteCard> GetMaxPlayerCards(DataEntities.Player player, DataEntities.Game game)
        {
            var ret = new List<DataEntities.WhiteCard>();

            var cardsBLL = new BusinessLogicLayer.WhiteCard(this.GetConnection());

            var currentCardIDs = player.WhiteCards.Select(x => x.WhiteCardID.GetValueOrDefault()).ToList();
            var selectedDeckIDs = game.Decks.Select(x => x.DeckID.GetValueOrDefault()).ToList();

            for (int i = 0; i < (WhiteCardsMax - player.WhiteCards.Count); i++)
            {
                var randomCard = cardsBLL.GetRandomWhiteCardBasedOnDecks(currentCardIDs, selectedDeckIDs);

                if (ret.Contains(randomCard))
                {
                    i--;
                }
                else
                {
                    ret.Add(randomCard);
                    currentCardIDs.Add(randomCard.WhiteCardID.GetValueOrDefault());
                }
            }

            return ret;
        }

        public List<DataEntities.WhiteCard> GetPlayerCards(int amount, DataEntities.Player player, DataEntities.Game game)
        {
            var ret = new List<DataEntities.WhiteCard>();

            var cardsBLL = new BusinessLogicLayer.WhiteCard(this.GetConnection());

            var currentCardIDs = player.WhiteCards.Select(x => x.WhiteCardID.GetValueOrDefault()).ToList();
            var selectedDeckIDs = game.Decks.Select(x => x.DeckID.GetValueOrDefault()).ToList();

            for (int i = 0; i < amount; i++)
            {
                var randomCard = cardsBLL.GetRandomWhiteCardBasedOnDecks(currentCardIDs, selectedDeckIDs);

                if (ret.Contains(randomCard))
                {
                    i--;
                }
                else
                {
                    ret.Add(randomCard);
                }
            }

            return ret;
        }

        #region DB

        public DataEntities.Game GetGameByGameID(int gameID, bool loadOtherInfo, bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.Games gamesDLL = new DatabaseLogicLayer.Games(GetConnection());

                return gamesDLL.GetGameByGameID(gameID, loadOtherInfo, isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }        
        
        public DataEntities.GameState GetLatestGameStateByGameID(int gameID)
        {
            try
            {
                DatabaseLogicLayer.Games gamesDLL = new DatabaseLogicLayer.Games(GetConnection());

                return gamesDLL.GetLatestGameStateByGameID(gameID);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public List<DataEntities.Game> GetGamesByUserID(int userID, bool loadOtherInfo, bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.Games personDLL = new DatabaseLogicLayer.Games(GetConnection());

                return personDLL.GetGamesByUserID(userID, loadOtherInfo, isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.Game>();
        }        
        
        public List<DataEntities.GameUser> GetGamesUsersByGameID(int gameID, bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.Games personDLL = new DatabaseLogicLayer.Games(GetConnection());

                return personDLL.GetGamesUsersByGameID(gameID, isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.GameUser>();
        }

        public List<DataEntities.GameDeck> GetGamesDecksByGameID(int gameID, bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.Games personDLL = new DatabaseLogicLayer.Games(GetConnection());

                return personDLL.GetGamesDecksByGameID(gameID, isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.GameDeck>();
        }

        public List<DataEntities.Game> GetAllGames(bool loadOtherInfo, bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.Games personDLL = new DatabaseLogicLayer.Games(GetConnection());

                return personDLL.GetAllGames(loadOtherInfo, isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.Game>();
        }

        public List<DataEntities.GameState> GetGameStatesByGameID(int gameStateID)
        {
            try
            {
                DatabaseLogicLayer.Games personDLL = new DatabaseLogicLayer.Games(GetConnection());

                return personDLL.GetGameStatesByGameID(gameStateID);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.GameState>();
        }

        public int? SaveGame(DataEntities.Game person)
        {
            try
            {
                DatabaseLogicLayer.Games personDLL = new DatabaseLogicLayer.Games(GetConnection());

                return personDLL.SaveGame(person);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public int? SaveGameState(DataEntities.GameState gameState)
        {
            try
            {
                DatabaseLogicLayer.Games personDLL = new DatabaseLogicLayer.Games(GetConnection());

                return personDLL.SaveGameState(gameState);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public long? SaveGameUser(DataEntities.GameUser person)
        {
            try
            {
                DatabaseLogicLayer.Games personDLL = new DatabaseLogicLayer.Games(GetConnection());

                return personDLL.SaveGameUser(person);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public long? SaveGameDeck(DataEntities.GameDeck person)
        {
            try
            {
                DatabaseLogicLayer.Games personDLL = new DatabaseLogicLayer.Games(GetConnection());

                return personDLL.SaveGameDeck(person);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        #endregion
    }
}
