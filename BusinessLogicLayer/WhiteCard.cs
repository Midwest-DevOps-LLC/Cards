using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class WhiteCard : BLLManager, IDisposable
    {
        public WhiteCard(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public WhiteCard(MySqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public List<DataEntities.WhiteCard> GetAllWhiteCards(bool loadDecks)
        {
            try
            {
                DatabaseLogicLayer.WhiteCards whiteCardDLL = new DatabaseLogicLayer.WhiteCards(GetConnection());

                var whiteCards = whiteCardDLL.GetAllWhiteCards(loadDecks);

                if (loadDecks)
                {
                    foreach (var whiteCard in whiteCards) //Maybe works?
                    {
                        FillInEntity(whiteCard);
                    }
                }

                return whiteCards;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.WhiteCard>();
        }

        public List<DataEntities.WhiteCard> GetWhiteCardsByUserID(int userID, bool loadDecks)
        {
            try
            {
                DatabaseLogicLayer.WhiteCards whiteCardDLL = new DatabaseLogicLayer.WhiteCards(GetConnection());

                var whiteCards = whiteCardDLL.GetWhiteCardsByUserID(userID, loadDecks);

                if (loadDecks)
                {
                    foreach (var whiteCard in whiteCards) //Maybe works?
                    {
                        FillInEntity(whiteCard);
                    }
                }

                return whiteCards;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.WhiteCard>();
        }

        public List<DataEntities.WhiteCard> GetAllWhiteCardsByDeckID(int deckID)
        {
            try
            {
                DatabaseLogicLayer.WhiteCards whiteCardDLL = new DatabaseLogicLayer.WhiteCards(GetConnection());

                var whiteCards = whiteCardDLL.GetAllWhiteCardsByDeckID(deckID);

                //if (loadDecks)
                //{
                //    foreach (var whiteCard in whiteCards) //Maybe works?
                //    {
                //        FillInEntity(whiteCard);
                //    }
                //}

                return whiteCards;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.WhiteCard>();
        }

        public DataEntities.WhiteCard GetWhiteCardByID(int id, bool loadDecks)
        {
            try
            {
                DatabaseLogicLayer.WhiteCards whiteCardDLL = new DatabaseLogicLayer.WhiteCards(GetConnection());

                var card = whiteCardDLL.GetWhiteCardByID(id);

                if (loadDecks)
                {
                    card = FillInEntity(card);
                }

                return card;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public DataEntities.WhiteCard GetRandomWhiteCard(int? duplicateID, bool loadDecks)
        {
            try
            {
                DatabaseLogicLayer.WhiteCards whiteCardDLL = new DatabaseLogicLayer.WhiteCards(GetConnection());

                var card = whiteCardDLL.GetRandomWhiteCard(duplicateID);

                if (loadDecks)
                {
                    card = FillInEntity(card);
                }

                return card;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public DataEntities.WhiteCard GetRandomWhiteCardBasedOnDecks(List<int> duplicateCardIDs, List<int> deckIDs)
        {
            try
            {
                DatabaseLogicLayer.WhiteCards whiteCardDLL = new DatabaseLogicLayer.WhiteCards(GetConnection());

                var card = whiteCardDLL.GetRandomWhiteCardBasedOnDecks(duplicateCardIDs, deckIDs);

                card = FillInEntity(card);

                return card;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public long? SaveWhiteCard(DataEntities.WhiteCard whiteCard)
        {
            try
            {
                DatabaseLogicLayer.WhiteCards whiteCardDLL = new DatabaseLogicLayer.WhiteCards(GetConnection());

                return whiteCardDLL.SaveWhiteCard(whiteCard);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        private DataEntities.WhiteCard FillInEntity(DataEntities.WhiteCard whiteCard)
        {
            //Fill in deck 
            using (BusinessLogicLayer.Deck deckBLL = new BusinessLogicLayer.Deck(GetConnection()))
            {
                whiteCard.Decks = deckBLL.GetAllDecksByWhiteCardID(whiteCard.WhiteCardID.Value);
            }

            return whiteCard;
        }
    }
}
