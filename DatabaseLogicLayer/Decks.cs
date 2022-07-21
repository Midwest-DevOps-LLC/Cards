using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DatabaseLogicLayer
{
    public class Decks : DBManager
    {

        #region BoringStuff

        string ConnectionString { get; set; }

        MySqlConnection sqlConnection { get; set; }

        public Decks(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public Decks(MySqlConnection sqlConnection)
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

        private string QueryString = @"SELECT * from card_deck";

        public List<DataEntities.Deck> GetAllDecks()
        {
            List<DataEntities.Deck> p = new List<DataEntities.Deck>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(QueryString, conn);
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

        public DataEntities.Deck GetDeckByID(int userID)
        {
            DataEntities.Deck person = new DataEntities.Deck();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(QueryString + " Where DeckID = @ID;", conn);

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

        public List<DataEntities.Deck> GetAllDecksByWhiteCardID(int cardID)
        {
            List<DataEntities.Deck> p = new List<DataEntities.Deck>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                var query = @"select * from white_cards as wc
                                join card_set_white as csw on wc.WhiteCardID = csw.CardID
                                join card_deck as cd on csw.DeckID = cd.DeckID
                                where wc.WhiteCardID = @ID;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", cardID);
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

        public List<DataEntities.Deck> GetAllDecksByBlackCardID(int cardID)
        {
            List<DataEntities.Deck> p = new List<DataEntities.Deck>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                var query = @"select * from black_cards as wc
                                join card_set_black as csw on wc.BlackCardID = csw.CardID
                                join card_deck as cd on csw.DeckID = cd.DeckID
                                where wc.BlackCardID = @ID;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", cardID);
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

        public long? SaveDeck(DataEntities.Deck p, bool refreshWhiteCardLinks, bool refreshBlackCardLinks)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                if (p.DeckID == null)
                {
                    sql = @"INSERT INTO `card_deck` (`DeckID`, `Name`, `Description`, `CreatedBy`) VALUES (NULL, @Name, @Description, @CreatedBy);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `card_deck` SET Name = @Name, Description = @Description, CreatedBy = @CreatedBy WHERE DeckID = @ID;";
                }

                cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ID", p.DeckID);
                cmd.Parameters.AddWithValue("@Name", p.Name);
                cmd.Parameters.AddWithValue("@Description", p.Description);
                cmd.Parameters.AddWithValue("@CreatedBy", p.CreatedBy);

                if (p.DeckID == null) //Get new id number
                {
                    p.DeckID = cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }

                if (refreshWhiteCardLinks)
                {
                    if (p.DeckID.HasValue)
                    {
                        sql = @"delete from card_set_white where DeckID = @ID";

                        cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@ID", p.DeckID);

                        cmd.ExecuteNonQuery();
                    }

                    foreach (var card in p.WhiteCards)
                    {
                        sql = @"insert into card_set_white (CardSetWhiteID, DeckID, CardID) values (NULL, @ID, @CardID);";

                        cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@ID", p.DeckID);
                        cmd.Parameters.AddWithValue("@CardID", card.WhiteCardID);

                        cmd.ExecuteNonQuery();
                    }
                }

                if (refreshBlackCardLinks)
                {
                    if (p.DeckID.HasValue)
                    {
                        sql = @"delete from card_set_black where DeckID = @ID";

                        cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@ID", p.DeckID);

                        cmd.ExecuteNonQuery();
                    }

                    foreach (var card in p.BlackCards)
                    {
                        sql = @"insert into card_set_black (CardSetBlackID, DeckID, CardID) values (NULL, @ID, @CardID);";

                        cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@ID", p.DeckID);
                        cmd.Parameters.AddWithValue("@CardID", card.BlackCardID);

                        cmd.ExecuteNonQuery();
                    }
                }

                return p.DeckID;
            }
        }

        static internal DataEntities.Deck ConvertMySQLToEntity(MySqlDataReader reader)
        {
            DataEntities.Deck p = new DataEntities.Deck();

            p.DeckID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "DeckID"));
            p.Name = DBUtilities.ReturnSafeString(reader, "Name");
            p.Description = DBUtilities.ReturnSafeString(reader, "Description");
            p.CreatedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CreatedBy"));

            return p;
        }
    }
}
