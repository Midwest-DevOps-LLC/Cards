using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DatabaseLogicLayer
{
    public class CardBattles : DBManager
    {

        #region BoringStuff

        string ConnectionString { get; set; }

        MySqlConnection sqlConnection { get; set; }

        public CardBattles(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public CardBattles(MySqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        internal MySqlConnection GetConnection()
        {
            if (sqlConnection == null)
            {
                return new MySqlConnection(this.ConnectionString);
            }
            else
            {
                return this.sqlConnection;
            }
        }

        #endregion

        public List<DataEntities.CardBattle> GetAllCardBattles()
        {
            List<DataEntities.CardBattle> p = new List<DataEntities.CardBattle>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM card_battle", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(ConvertMySQLToEntity(reader));
                    }
                }
            }

            return p;
        }

        public List<DataEntities.CardBattle.CardBattleIndex.CardBattleRank> GetAllCardBattleRanking()
        {
            List<DataEntities.CardBattle.CardBattleIndex.CardBattleRank> p = new List<DataEntities.CardBattle.CardBattleIndex.CardBattleRank>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT WinningCard, wc.Text, COUNT(WinningCard) as Rank FROM `card_battle` as cb join white_cards as wc on cb.WinningCard = wc.WhiteCardID where WinningCard IS NOT NULL group by WinningCard HAVING COUNT(WinningCard) > 1 ORDER BY Rank DESC LIMIT 10;", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(DataEntities.CardBattle.CardBattleIndex.CardBattleRank.ConvertMySQLToEntity(reader));
                    }
                }
            }

            return p;
        }

        public long? SaveCardBattle(DataEntities.CardBattle cardBattle)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (cardBattle.CardBattleID == null)
                {
                    sql = @"INSERT INTO `card_battle` (`CardBattleID`, `UserID`, `CardID1`, `CardID2`, `WinningCard`) VALUES (NULL, @UserID, @CardID1, @CardID2, NULL);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `card_battle` SET CardBattleID = @ID, UserID = @UserID, CardID1 = @CardID1, CardID2 = @CardID2, WinningCard = @WinningCard WHERE CardBattleID = @ID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ID", cardBattle.CardBattleID);
                cmd.Parameters.AddWithValue("@UserID", cardBattle.UserID);
                cmd.Parameters.AddWithValue("@CardID1", cardBattle.Card1.WhiteCardID);
                cmd.Parameters.AddWithValue("@CardID2", cardBattle.Card2.WhiteCardID);
                cmd.Parameters.AddWithValue("@WinningCard", cardBattle.WinningCard != null ? cardBattle.WinningCard.WhiteCardID : (int?)null);

                if (cardBattle.CardBattleID == null) //Get new id number
                {
                    return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }
                else //Return id if worked
                {
                    cmd.ExecuteScalar();

                    return cardBattle.CardBattleID;
                }
            }
        }

        public DataEntities.CardBattle GetCardBattleByID(int userID)
        {
            DataEntities.CardBattle person = new DataEntities.CardBattle();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM card_battle Where CardBattleID = @ID;", conn);

                cmd.Parameters.AddWithValue("@ID", userID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return ConvertMySQLToEntity(reader);
                    }

                    return null;
                }
            }
        }

        internal DataEntities.CardBattle ConvertMySQLToEntity(MySqlDataReader reader)
        {
            DataEntities.CardBattle p = new DataEntities.CardBattle();

            p.CardBattleID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CardBattleID"));
            p.UserID = DBUtilities.ReturnSafeInt(reader, "UserID");

            var cardID1 = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CardID1"));
            var cardID2 = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CardID2"));
            var winningCardID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "WinningCard"));

            p.Card1 = new DataEntities.WhiteCard() { WhiteCardID = cardID1 };
            p.Card2 = new DataEntities.WhiteCard() { WhiteCardID = cardID2 };
            p.WinningCard = new DataEntities.WhiteCard() { WhiteCardID = winningCardID };

            return p;
        }
    }
}
