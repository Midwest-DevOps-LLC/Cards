using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class Deck : BLLManager, IDisposable
    {
        public Deck(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public Deck(MySqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public List<DataEntities.Deck> GetAllDecks(bool fillIn)
        {
            try
            {
                DatabaseLogicLayer.Decks deckDLL = new DatabaseLogicLayer.Decks(GetConnection());

                var decks = deckDLL.GetAllDecks();

                if (fillIn)
                {
                    foreach (var deck in decks)
                    {
                        FillInEntity(deck);
                    }
                }

                return decks;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.Deck>();
        }

        public List<DataEntities.Deck> GetAllDecksByWhiteCardID(int whiteCardID)
        {
            try
            {
                DatabaseLogicLayer.Decks deckDLL = new DatabaseLogicLayer.Decks(GetConnection());

                return deckDLL.GetAllDecksByWhiteCardID(whiteCardID);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.Deck>();
        }

        public List<DataEntities.Deck> GetAllDecksByBlackCardID(int blackCardID)
        {
            try
            {
                DatabaseLogicLayer.Decks deckDLL = new DatabaseLogicLayer.Decks(GetConnection());

                return deckDLL.GetAllDecksByBlackCardID(blackCardID);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.Deck>();
        }

        public DataEntities.Deck GetDeckByID(int id, bool fillIn)
        {
            try
            {
                DatabaseLogicLayer.Decks deckDLL = new DatabaseLogicLayer.Decks(GetConnection());

                var deck = deckDLL.GetDeckByID(id);

                if (fillIn)
                {
                    deck = FillInEntity(deck);
                }

                return FillInEntity(deck);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public long? SaveCardBattle(DataEntities.Deck cardBattle, bool refreshWhiteCards, bool refreshBlackCards)
        {
            try
            {
                DatabaseLogicLayer.Decks cardBattleDLL = new DatabaseLogicLayer.Decks(GetConnection());

                return cardBattleDLL.SaveDeck(cardBattle, refreshWhiteCards, refreshBlackCards);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        private DataEntities.Deck FillInEntity(DataEntities.Deck deck)
        {
            //Fill in deck 
            using (BusinessLogicLayer.WhiteCard deckBLL = new BusinessLogicLayer.WhiteCard(GetConnection()))
            {
                deck.WhiteCards = deckBLL.GetAllWhiteCardsByDeckID(deck.DeckID.Value);

                var blackDeckBLL = new BusinessLogicLayer.BlackCard(deckBLL.GetConnection());
                deck.BlackCards = blackDeckBLL.GetAllBlackCardsByDeckID(deck.DeckID.Value);
            }

            return deck;
        }
    }
}
