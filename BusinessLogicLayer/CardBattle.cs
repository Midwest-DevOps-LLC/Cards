using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class CardBattle : BLLManager, IDisposable
    {
        public CardBattle(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public CardBattle(MySqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public List<DataEntities.CardBattle> GetAllCardBattles()
        {
            try
            {
                DatabaseLogicLayer.CardBattles cardBattleDLL = new DatabaseLogicLayer.CardBattles(GetConnection());

                return cardBattleDLL.GetAllCardBattles();
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.CardBattle>();
        }

        public List<DataEntities.CardBattle.CardBattleIndex.CardBattleRank> GetAllCardBattleRanking()
        {
            try
            {
                DatabaseLogicLayer.CardBattles cardBattleDLL = new DatabaseLogicLayer.CardBattles(GetConnection());

                return cardBattleDLL.GetAllCardBattleRanking();
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.CardBattle.CardBattleIndex.CardBattleRank>();
        }

        public DataEntities.CardBattle GetCardBattle(int ID)
        {
            try
            {
                var cardBattleDLL = new DatabaseLogicLayer.CardBattles(GetConnection());
                var whiteCardDLL = new DatabaseLogicLayer.WhiteCards(GetConnection());

                var cardBattle = cardBattleDLL.GetCardBattleByID(ID);

                //Fill in white cards
                cardBattle.Card1 = whiteCardDLL.GetWhiteCardByID(cardBattle.Card1.WhiteCardID.Value);
                cardBattle.Card2 = whiteCardDLL.GetWhiteCardByID(cardBattle.Card2.WhiteCardID.Value);
                cardBattle.WinningCard = whiteCardDLL.GetWhiteCardByID(cardBattle.WinningCard.WhiteCardID.Value);

                return cardBattle;
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public DataEntities.CardBattle CreateCardBattle(int? userID)
        {
            try
            {
                var cardBattleDLL = new DatabaseLogicLayer.CardBattles(GetConnection());
                var whiteCardBLL = new BusinessLogicLayer.WhiteCard(GetConnection());

                var cardBattle = new DataEntities.CardBattle();

                var randomCard1 = whiteCardBLL.GetRandomWhiteCard(null, true);
                var randomCard2 = whiteCardBLL.GetRandomWhiteCard(randomCard1.WhiteCardID, true);

                cardBattle.Card1 = randomCard1;
                cardBattle.Card2 = randomCard2;

                cardBattle.UserID = userID;

                var saveResult = cardBattleDLL.SaveCardBattle(cardBattle);

                if (saveResult == null)
                {
                    return null;
                }
                else
                {
                    cardBattle.CardBattleID = saveResult.GetValueOrDefault();

                    return cardBattle;
                }
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public long? SaveCardBattle(DataEntities.CardBattle cardBattle)
        {
            try
            {
                DatabaseLogicLayer.CardBattles cardBattleDLL = new DatabaseLogicLayer.CardBattles(GetConnection());

                return cardBattleDLL.SaveCardBattle(cardBattle);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }
    }
}
