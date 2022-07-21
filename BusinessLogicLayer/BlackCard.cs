using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class BlackCard : BLLManager, IDisposable
    {
        public BlackCard(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public BlackCard(MySqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public List<DataEntities.BlackCard> GetAllBlackCards(bool loadDecks)
        {
            try
            {
                DatabaseLogicLayer.BlackCards whiteCardDLL = new DatabaseLogicLayer.BlackCards(GetConnection());

                var blackCards = whiteCardDLL.GetAllBlackCards(loadDecks);

                if (loadDecks)
                {
                    foreach (var whiteCard in blackCards) //Maybe works?
                    {
                        FillInEntity(whiteCard);
                    }
                }

                return blackCards;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.BlackCard>();
        }

        public List<DataEntities.BlackCard> GetBlackCardsByUserID(int userID, bool loadDecks)
        {
            try
            {
                DatabaseLogicLayer.BlackCards whiteCardDLL = new DatabaseLogicLayer.BlackCards(GetConnection());

                var blackCards = whiteCardDLL.GetBlackCardsByUserID(userID, loadDecks);

                if (loadDecks)
                {
                    foreach (var whiteCard in blackCards) //Maybe works?
                    {
                        FillInEntity(whiteCard);
                    }
                }

                return blackCards;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.BlackCard>();
        }

        public List<DataEntities.BlackCard> GetAllBlackCardsByDeckID(int deckID)
        {
            try
            {
                DatabaseLogicLayer.BlackCards whiteCardDLL = new DatabaseLogicLayer.BlackCards(GetConnection());

                var blackCards = whiteCardDLL.GetAllBlackCardsByDeckID(deckID);

                //if (loadDecks)
                //{
                //    foreach (var whiteCard in whiteCards) //Maybe works?
                //    {
                //        FillInEntity(whiteCard);
                //    }
                //}

                return blackCards;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.BlackCard>();
        }

        public DataEntities.BlackCard GetBlackCardByID(int id, bool loadDecks)
        {
            try
            {
                DatabaseLogicLayer.BlackCards whiteCardDLL = new DatabaseLogicLayer.BlackCards(GetConnection());

                var card = whiteCardDLL.GetBlackCardByID(id);

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

        public DataEntities.BlackCard GetRandomBlackCard(int? duplicateID, bool loadDecks)
        {
            try
            {
                DatabaseLogicLayer.BlackCards whiteCardDLL = new DatabaseLogicLayer.BlackCards(GetConnection());

                var card = whiteCardDLL.GetRandomBlackCard(duplicateID);

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

        public DataEntities.BlackCard GetRandomBlackCardBasedOnDecks(List<int> duplicateCardIDs, List<int> deckIDs, bool onlyPick1)
        {
            try
            {
                DatabaseLogicLayer.BlackCards whiteCardDLL = new DatabaseLogicLayer.BlackCards(GetConnection());

                var card = whiteCardDLL.GetRandomBlackCardBasedOnDecks(duplicateCardIDs, deckIDs, onlyPick1);

                card = FillInEntity(card);

                return card;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public long? SaveBlackCard(DataEntities.BlackCard blackCard)
        {
            try
            {
                DatabaseLogicLayer.BlackCards whiteCardDLL = new DatabaseLogicLayer.BlackCards(GetConnection());

                return whiteCardDLL.SaveBlackCard(blackCard);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        private DataEntities.BlackCard FillInEntity(DataEntities.BlackCard whiteCard)
        {
            //Fill in deck 
            using (BusinessLogicLayer.Deck deckBLL = new BusinessLogicLayer.Deck(GetConnection()))
            {
                whiteCard.Decks = deckBLL.GetAllDecksByBlackCardID(whiteCard.BlackCardID.Value);
            }

            return whiteCard;
        }
    }
}
