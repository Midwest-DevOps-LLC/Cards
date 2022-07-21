using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace DatabaseLogicLayer
{
    public class BlackCards : DBManager
    {

        #region BoringStuff

        string ConnectionString { get; set; }

        MySqlConnection sqlConnection { get; set; }

        public BlackCards(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public BlackCards(MySqlConnection sqlConnection)
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

        private string QueryStringDeck = @"SELECT wc.*, cd.*, cd.Name as DeckName FROM black_cards as wc join card_set_black as csw on csw.CardID = wc.BlackCardID join card_deck as cd on csw.DeckID = cd.DeckID";
        private string QueryString = "SELECT *, '' as DeckName from black_cards";

        public List<DataEntities.BlackCard> GetAllBlackCards(bool loadDecks)
        {
            List<DataEntities.BlackCard> p = new List<DataEntities.BlackCard>();

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

        public List<DataEntities.BlackCard> GetBlackCardsByUserID(int userID, bool loadDecks)
        {
            List<DataEntities.BlackCard> p = new List<DataEntities.BlackCard>();

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

        public List<DataEntities.BlackCard> GetAllBlackCardsByDeckID(int deckID)
        {
            List<DataEntities.BlackCard> p = new List<DataEntities.BlackCard>();

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

        public DataEntities.BlackCard GetBlackCardByID(int id)
        {
            DataEntities.BlackCard p = new DataEntities.BlackCard();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(QueryString + " where BlackCardID = @ID", conn);
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

        public DataEntities.BlackCard GetRandomBlackCard(int? duplicateID)
        {
            DataEntities.BlackCard p = new DataEntities.BlackCard();

            if (duplicateID.HasValue == false)
            {
                duplicateID = 0;
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = @"SELECT wc.*, cd.Name as DeckName FROM black_cards as wc
   	                            join card_set_black csw on wc.BlackCardID = csw.CardID
                                join card_deck cd on csw.DeckID = cd.DeckID
                                WHERE wc.BlackCardID <> @DuplicateID
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

        public DataEntities.BlackCard GetRandomBlackCardBasedOnDecks(List<int> duplicateCardIDs, List<int> deckIDs, bool onlyPick1)
        {
            DataEntities.BlackCard p = new DataEntities.BlackCard();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = @"SELECT bc.*, cd.Name as DeckName FROM black_cards as bc 
   	                            join card_set_black csb on bc.BlackCardID = csb.CardID 
                                join card_deck cd on csb.DeckID = cd.DeckID and FIND_IN_SET(cd.DeckID, @DeckIDs) != 0 
                                WHERE cd.DeckID <> 0 " +
                                (duplicateCardIDs.Any() ? " AND bc.BlackCardID NOT IN (@DuplicateIDs) " : "") +
                                (onlyPick1 ? " AND bc.Pick = 1 " : "") +
                                @" ORDER BY RAND() 
                                LIMIT 1";
                 
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                if (duplicateCardIDs.Any())
                {
                    cmd.Parameters.AddWithValue("@DuplicateIDs", duplicateCardIDs);
                }

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

        public long? SaveBlackCard(DataEntities.BlackCard p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (p.BlackCardID == null)
                {
                    sql = @"INSERT INTO `black_cards` (`BlackCardID`, `Draw`, `Pick`, `Text`, `CreatedBy`) VALUES (NULL, @Draw, @Pick, @Text, @CreatedBy);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `black_cards` SET Draw = @Draw, Pick = @Pick, Text = @Text, CreatedBy = @CreatedBy WHERE BlackCardID = @ID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ID", p.BlackCardID);
                cmd.Parameters.AddWithValue("@Draw", p.Draw);
                cmd.Parameters.AddWithValue("@Pick", p.Pick);
                cmd.Parameters.AddWithValue("@Text", p.Text);
                cmd.Parameters.AddWithValue("@CreatedBy", p.CreatedBy);

                if (p.BlackCardID == null) //Get new id number
                {
                    return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }
                else //Return id if worked
                {
                    cmd.ExecuteScalar();

                    return p.BlackCardID;
                }
            }
        }

        internal DataEntities.BlackCard ConvertMySQLToEntity(MySqlDataReader reader, MySqlCommand cmd)
        {
            DataEntities.BlackCard p = new DataEntities.BlackCard();

            p.BlackCardID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "BlackCardID"));
            p.Draw = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "Draw"));
            p.Pick = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "Pick"));
            p.CreatedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CreatedBy"));
            p.Text = DBUtilities.ReturnSafeString(reader, "Text");
            p.DeckName = DBUtilities.ReturnSafeString(reader, "DeckName");

            return p;
        }
    }
}
