using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DatabaseLogicLayer
{
    public class WhiteCards : DBManager
    {

        #region BoringStuff

        string ConnectionString { get; set; }

        MySqlConnection sqlConnection { get; set; }

        public WhiteCards(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public WhiteCards(MySqlConnection sqlConnection)
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

        private string QueryStringDeck = @"SELECT wc.*, cd.*, cd.Name as DeckName FROM white_cards as wc join card_set_white as csw on csw.CardID = wc.WhiteCardID join card_deck as cd on csw.DeckID = cd.DeckID";
        private string QueryString = "SELECT *, '' as DeckName from white_cards";

        public List<DataEntities.WhiteCard> GetAllWhiteCards(bool loadDecks)
        {
            List<DataEntities.WhiteCard> p = new List<DataEntities.WhiteCard>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(loadDecks ? QueryStringDeck : QueryString, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(ConvertMySQLToEntity(reader, cmd));
                    }
                }
            }

            return p;
        }

        public List<DataEntities.WhiteCard> GetWhiteCardsByUserID(int userID, bool loadDecks)
        {
            List<DataEntities.WhiteCard> p = new List<DataEntities.WhiteCard>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand((loadDecks ? QueryStringDeck : QueryString) + " where createdby = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", userID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(ConvertMySQLToEntity(reader, cmd));
                    }
                }
            }

            return p;
        }

        public List<DataEntities.WhiteCard> GetAllWhiteCardsByDeckID(int deckID)
        {
            List<DataEntities.WhiteCard> p = new List<DataEntities.WhiteCard>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(QueryStringDeck + " where cd.DeckID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", deckID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(ConvertMySQLToEntity(reader, cmd));
                    }
                }
            }

            return p;
        }

        public DataEntities.WhiteCard GetWhiteCardByID(int id)
        {
            DataEntities.WhiteCard p = new DataEntities.WhiteCard();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(QueryStringDeck + " where wc.WhiteCardID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p = ConvertMySQLToEntity(reader, cmd);
                    }
                }
            }

            return p;
        }

        public DataEntities.WhiteCard GetRandomWhiteCard(int? duplicateID)
        {
            DataEntities.WhiteCard p = new DataEntities.WhiteCard();

            if (duplicateID.HasValue == false)
            {
                duplicateID = 0;
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = @"SELECT wc.*, cd.Name as DeckName FROM white_cards as wc
   	                            join card_set_white csw on wc.WhiteCardID = csw.CardID
                                join card_deck cd on csw.DeckID = cd.DeckID
                                WHERE wc.WhiteCardID <> @DuplicateID
                                ORDER BY RAND()
                                LIMIT 1";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DuplicateID", duplicateID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p = ConvertMySQLToEntity(reader, cmd);
                    }
                }
            }

            return p;
        }

        public DataEntities.WhiteCard GetRandomWhiteCardBasedOnDecks(List<int> duplicateCardIDs, List<int> deckIDs)
        {
            DataEntities.WhiteCard p = new DataEntities.WhiteCard();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = @"SELECT wc.*, cd.Name as DeckName FROM white_cards as wc
   	                            join card_set_white csw on wc.WhiteCardID = csw.CardID
                                join card_deck cd on csw.DeckID = cd.DeckID
                                WHERE FIND_IN_SET(wc.WhiteCardID, @DuplicateIDs) = 0
                                AND FIND_IN_SET(cd.DeckID, @DeckIDs) != 0
                                ORDER BY RAND()
                                LIMIT 1";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DuplicateIDs", string.Join(",", duplicateCardIDs));
                cmd.Parameters.AddWithValue("@DeckIDs", string.Join(",", deckIDs));
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p = ConvertMySQLToEntity(reader, cmd);
                    }
                }
            }

            return p;
        }

        //public DataEntities.WhiteCard GetWhiteCardByID(int userID)
        //{
        //    DataEntities.WhiteCard person = new DataEntities.WhiteCard();

        //    using (MySqlConnection conn = GetConnection())
        //    {
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand("SELECT * FROM white_cards Where WhiteCardID = @ID;", conn);

        //        cmd.Parameters.AddWithValue("@ID", userID);
        //        using (MySqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                return ConvertMySQLToEntity(reader, cmd);
        //            }

        //            return null;
        //        }
        //    }
        //}

        //public DataEntities.User GetUserByEmail(string email)
        //{
        //    DataEntities.User person = new DataEntities.User();

        //    using (MySqlConnection conn = GetConnection())
        //    {
        //        conn.Open();
        //        MySqlCommand cmd = new MySqlCommand("SELECT * FROM user Where Email = @ID;", conn);

        //        cmd.Parameters.AddWithValue("@ID", email);
        //        using (MySqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                return ConvertMySQLToEntity(reader);
        //            }

        //            return null;
        //        }
        //    }
        //}

        //public DataEntities.User GetUserByUsername(string userName)
        //{
        //    DataEntities.User person = new DataEntities.User();

        //    using (MySqlConnection conn = GetConnection())
        //    {
        //        conn.Open();
        //        MySqlCommand cmd = new MySqlCommand("SELECT * FROM user Where Username = @Username;", conn);

        //        cmd.Parameters.AddWithValue("@Username", userName);
        //        using (MySqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                return ConvertMySQLToEntity(reader);
        //            }

        //            return null;
        //        }
        //    }
        //}

        //public DataEntities.User GetUserByUUID(string UUID)
        //{
        //    DataEntities.User person = new DataEntities.User();

        //    using (MySqlConnection conn = GetConnection())
        //    {
        //        conn.Open();
        //        MySqlCommand cmd = new MySqlCommand("SELECT * FROM user Where UUID = @UUID;", conn);

        //        cmd.Parameters.AddWithValue("@UUID", UUID);
        //        using (MySqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                return ConvertMySQLToEntity(reader);
        //            }

        //            return null;
        //        }
        //    }
        //}

        public long? SaveWhiteCard(DataEntities.WhiteCard p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (p.WhiteCardID == null)
                {
                    sql = @"INSERT INTO `white_cards` (`WhiteCardID`, `Text`, `CreatedBy`) VALUES (NULL, @Text, @CreatedBy);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `white_cards` SET Text = @Text, CreatedBy = @CreatedBy WHERE WhiteCardID = @ID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ID", p.WhiteCardID);
                cmd.Parameters.AddWithValue("@Text", p.Text);
                cmd.Parameters.AddWithValue("@CreatedBy", p.CreatedBy);

                if (p.WhiteCardID == null) //Get new id number
                {
                    return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }
                else //Return id if worked
                {
                    cmd.ExecuteScalar();

                    return p.WhiteCardID;
                }
            }
        }

        internal DataEntities.WhiteCard ConvertMySQLToEntity(MySqlDataReader reader, MySqlCommand cmd)
        {
            DataEntities.WhiteCard p = new DataEntities.WhiteCard();

            p.WhiteCardID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "WhiteCardID"));
            p.CreatedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CreatedBy"));
            p.Text = DBUtilities.ReturnSafeString(reader, "Text");
            p.DeckName = DBUtilities.ReturnSafeString(reader, "DeckName");

            return p;
        }
    }
}
