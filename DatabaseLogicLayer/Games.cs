using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseLogicLayer
{
    public class Games : DBManager
    {
        #region Initializer

        string ConnectionString { get; set; }

        MySqlConnection sqlConnection { get; set; }

        public Games(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public Games(MySqlConnection sqlConnection)
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

        public DataEntities.Game GetGameByGameID(int gameID, bool loadOtherInfo, bool? isActive)
        {
            var ret = new DataEntities.Game();

            string sql = "";

            if (isActive == true)
            {
                sql = "SELECT * FROM game WHERE GameID = @GameID AND Active = 1;";
            }
            else if (isActive == false)
            {
                sql = "SELECT * FROM game WHERE GameID = @GameID AND Active = 0;";
            }
            else
            {
                sql = "SELECT * FROM game WHERE GameID = @GameID;";
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@GameID", gameID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret = ConvertMySQLToEntity(reader);
                    }
                }
            }

            if (ret != null && loadOtherInfo)
            {
                //Users
                if (isActive == true)
                {
                    sql = "SELECT * FROM game_user WHERE GameID = @GameID AND Active = 1;";
                }
                else if (isActive == false)
                {
                    sql = "SELECT * FROM game_user WHERE GameID = @GameID AND Active = 0;";
                }
                else
                {
                    sql = "SELECT * FROM game_user WHERE GameID = @GameID;";
                }

                List<int> GameUsers = new List<int>();
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@GameID", gameID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GameUsers.Add(Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "UserID")));
                        }
                    }
                }

                var userDAL = new DatabaseLogicLayer.Users(this.sqlConnection);

                var userEntities = new List<DataEntities.User>();

                foreach (var user in GameUsers)
                {
                    userEntities.Add(userDAL.GetUserByUserID(user, true));
                }

                ret.Users = userEntities;

                //Decks
                if (isActive == true)
                {
                    sql = "SELECT * FROM game_deck WHERE GameID = @GameID AND Active = 1;";
                }
                else if (isActive == false)
                {
                    sql = "SELECT * FROM game_deck WHERE GameID = @GameID AND Active = 0;";
                }
                else
                {
                    sql = "SELECT * FROM game_deck WHERE GameID = @GameID;";
                }

                List<int> GameDecks = new List<int>();
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@GameID", gameID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GameDecks.Add(Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "DeckID")));
                        }
                    }
                }

                var deckDAL = new DatabaseLogicLayer.Decks(this.sqlConnection);

                var deckEntities = new List<DataEntities.Deck>();

                foreach (var deck in GameDecks)
                {
                    deckEntities.Add(deckDAL.GetDeckByID(deck));
                }

                ret.Decks = deckEntities;

                //Game Options
                if (isActive == true)
                {
                    sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID AND Active = 1;";
                }
                else if (isActive == false)
                {
                    sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID AND Active = 0;";
                }
                else
                {
                    sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID;";
                }

                List<int> GameOptionsPicked = new List<int>();
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@GameID", gameID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GameOptionsPicked.Add(Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameOptionID")));
                        }
                    }
                }

                var gameOptionDAL = new DatabaseLogicLayer.GameOptions(this.sqlConnection);

                var gameOptionEntities = new List<DataEntities.GameOption>();

                foreach (var optionID in GameOptionsPicked)
                {
                    gameOptionEntities.Add(gameOptionDAL.GetGameOptionByGameOptionID(optionID, true));
                }

                ret.GameOptions = gameOptionEntities;

                return ret;
            }
            else if (ret != null)
            {
                return ret;
            }


            return null;
        }

        //public DataEntities.User GetGameByUsername(string username, bool? isActive)
        //{
        //    using (MySqlConnection conn = GetConnection())
        //    {
        //        conn.Open();

        //        string sql = "";

        //        if (isActive == true)
        //        {
        //            sql = "SELECT * FROM user WHERE Username LIKE @Username AND Active = 1;";
        //        }
        //        else if (isActive == false)
        //        {
        //            sql = "SELECT * FROM user WHERE Username LIKE @Username AND Active = 0;";
        //        }
        //        else
        //        {
        //            sql = "SELECT * FROM user WHERE Username LIKE @Username;";
        //        }

        //        MySqlCommand cmd = new MySqlCommand(sql, conn);

        //        cmd.Parameters.AddWithValue("@Username", username.ToUpper());
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

        //public DataEntities.User GetUserByUserAuthID(int userAuthID, bool? isActive)
        //{
        //    using (MySqlConnection conn = GetConnection())
        //    {
        //        conn.Open();

        //        string sql = "";

        //        if (isActive == true)
        //        {
        //            sql = "SELECT * FROM user WHERE UserAuthID = @UserAuthID AND Active = 1;";
        //        }
        //        else if (isActive == false)
        //        {
        //            sql = "SELECT * FROM user WHERE UserAuthID = @UserAuthID AND Active = 0;";
        //        }
        //        else
        //        {
        //            sql = "SELECT * FROM user WHERE UserAuthID = @UserAuthID;";
        //        }

        //        MySqlCommand cmd = new MySqlCommand(sql, conn);

        //        cmd.Parameters.AddWithValue("@UserAuthID", userAuthID);
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

        public List<DataEntities.Game> GetGamesByUserID(int userID, bool loadOtherInfo, bool? isActive)
        {
            List<DataEntities.Game> ret = new List<DataEntities.Game>();

            string sql = "";

            if (isActive == true)
            {
                sql = "SELECT * FROM game as g left join game_user as gu on g.GameID = gu.GameID WHERE g.CreatedBy = @UserID AND g.Active = 1;";
            }
            else if (isActive == false)
            {
                sql = "SELECT * FROM game as g left join game_user as gu on g.GameID = gu.GameID WHERE g.CreatedBy = @UserID AND g.Active = 0;";
            }
            else
            {
                sql = "SELECT * FROM game as g left join game_user as gu on g.GameID = gu.GameID WHERE g.CreatedBy = @UserID;";
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(ConvertMySQLToEntity(reader));
                    }
                }
            }

            if (ret != null && loadOtherInfo)
            {
                foreach (var game in ret)
                {
                    if (isActive == true)
                    {
                        sql = "SELECT * FROM game_user WHERE GameID = @GameID AND Active = 1;";
                    }
                    else if (isActive == false)
                    {
                        sql = "SELECT * FROM game_user WHERE GameID = @GameID AND Active = 0;";
                    }
                    else
                    {
                        sql = "SELECT * FROM game_user WHERE GameID = @GameID;";
                    }

                    List<int> GameUsers = new List<int>();
                    using (MySqlConnection conn = GetConnection())
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@GameID", game.GameID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GameUsers.Add(Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "UserID")));
                            }
                        }
                    }

                    var userDAL = new DatabaseLogicLayer.Users(this.sqlConnection);

                    var userEntities = new List<DataEntities.User>();

                    foreach (var user in GameUsers)
                    {
                        userEntities.Add(userDAL.GetUserByUserID(user, true));
                    }

                    game.Users = userEntities;

                    if (isActive == true)
                    {
                        sql = "SELECT * FROM game_deck WHERE GameID = @GameID AND Active = 1;";
                    }
                    else if (isActive == false)
                    {
                        sql = "SELECT * FROM game_deck WHERE GameID = @GameID AND Active = 0;";
                    }
                    else
                    {
                        sql = "SELECT * FROM game_deck WHERE GameID = @GameID;";
                    }

                    List<int> GameDecks = new List<int>();
                    using (MySqlConnection conn = GetConnection())
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@GameID", game.GameID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GameDecks.Add(Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "DeckID")));
                            }
                        }
                    }

                    var deckDAL = new DatabaseLogicLayer.Decks(this.sqlConnection);

                    var deckEntities = new List<DataEntities.Deck>();

                    foreach (var deck in GameDecks)
                    {
                        deckEntities.Add(deckDAL.GetDeckByID(deck));
                    }

                    game.Decks = deckEntities;

                    //Game Options
                    if (isActive == true)
                    {
                        sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID AND Active = 1;";
                    }
                    else if (isActive == false)
                    {
                        sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID AND Active = 0;";
                    }
                    else
                    {
                        sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID;";
                    }

                    List<int> GameOptionsPicked = new List<int>();
                    using (MySqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@GameID", game.GameID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GameOptionsPicked.Add(Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "UserID")));
                            }
                        }
                    }

                    var gameOptionDAL = new DatabaseLogicLayer.GameOptions(this.sqlConnection);

                    var gameOptionEntities = new List<DataEntities.GameOption>();

                    foreach (var optionID in GameOptionsPicked)
                    {
                        gameOptionEntities.Add(gameOptionDAL.GetGameOptionByGameOptionID(optionID, true));
                    }

                    game.GameOptions = gameOptionEntities;

                    return ret;
                }
            }
            else if (ret != null)
            {
                return ret;
            }

            return ret;
        }

        public List<DataEntities.GameUser> GetGamesUsersByGameID(int gameID, bool? isActive)
        {
            List<DataEntities.GameUser> p = new List<DataEntities.GameUser>();

            string sql = "";

            if (isActive == true)
            {
                sql = "SELECT * FROM game_user WHERE GameID = @GameID AND Active = 1;";
            }
            else if (isActive == false)
            {
                sql = "SELECT * FROM game_user WHERE GameID = @GameID AND Active = 0;";
            }
            else
            {
                sql = "SELECT * FROM game_user WHERE GameID = @GameID;";
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@GameID", gameID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(ConvertMySQLToEntityGU(reader));
                    }
                }
            }

            return p;
        }

        public List<DataEntities.GameDeck> GetGamesDecksByGameID(int gameID, bool? isActive)
        {
            List<DataEntities.GameDeck> p = new List<DataEntities.GameDeck>();

            string sql = "";

            if (isActive == true)
            {
                sql = "SELECT * FROM game_deck WHERE GameID = @GameID AND Active = 1;";
            }
            else if (isActive == false)
            {
                sql = "SELECT * FROM game_deck WHERE GameID = @GameID AND Active = 0;";
            }
            else
            {
                sql = "SELECT * FROM game_deck WHERE GameID = @GameID;";
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@GameID", gameID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(ConvertMySQLToEntityGD(reader));
                    }
                }
            }

            return p;
        }

        public List<DataEntities.GameOptionPicked> GetGameOptionsPickedByGameID(int gameID, bool? isActive)
        {
            List<DataEntities.GameOptionPicked> p = new List<DataEntities.GameOptionPicked>();

            string sql = "";

            if (isActive == true)
            {
                sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID AND Active = 1;";
            }
            else if (isActive == false)
            {
                sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID AND Active = 0;";
            }
            else
            {
                sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID;";
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@GameID", gameID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(ConvertMySQLToEntityGOP(reader));
                    }
                }
            }

            return p;
        }

        public List<DataEntities.GameState> GetGameStatesByGameID(int gameID)
        {
            List<DataEntities.GameState> p = new List<DataEntities.GameState>();

            string sql = "";

            sql = "SELECT * FROM game_state WHERE GameID = @GameID;";

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@GameID", gameID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Add(ConvertMySQLToEntityGS(reader));
                    }
                }
            }

            return p;
        }

        public DataEntities.GameState GetLatestGameStateByGameID(int gameID)
        {
            DataEntities.GameState p = null;

            string sql = "";

            sql = "SELECT * FROM game_state WHERE GameID = @GameID ORDER BY GameStateID DESC LIMIT 1;";

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@GameID", gameID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p = ConvertMySQLToEntityGS(reader);
                    }
                }
            }

            return p;
        }

        public List<DataEntities.Game> GetAllGames(bool loadOtherInfo, bool? isActive)
        {
            List<DataEntities.Game> ret = new List<DataEntities.Game>();

            string sql = "";

            if (isActive == true)
            {
                sql = "SELECT * FROM game WHERE Active = 1;";
            }
            else if (isActive == false)
            {
                sql = "SELECT * FROM game WHERE Active = 0;";
            }
            else
            {
                sql = "SELECT * FROM game;";
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(ConvertMySQLToEntity(reader));
                    }
                }
            }

            if (ret != null && loadOtherInfo)
            {
                foreach (var game in ret)
                {
                    if (isActive == true)
                    {
                        sql = "SELECT * FROM game_user WHERE GameID = @GameID AND Active = 1;";
                    }
                    else if (isActive == false)
                    {
                        sql = "SELECT * FROM game_user WHERE GameID = @GameID AND Active = 0;";
                    }
                    else
                    {
                        sql = "SELECT * FROM game_user WHERE GameID = @GameID;";
                    }

                    List<int> GameUsers = new List<int>();
                    using (MySqlConnection conn = GetConnection())
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@GameID", game.GameID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GameUsers.Add(Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "UserID")));
                            }
                        }
                    }

                    var userDAL = new DatabaseLogicLayer.Users(this.sqlConnection);

                    var userEntities = new List<DataEntities.User>();

                    foreach (var user in GameUsers)
                    {
                        userEntities.Add(userDAL.GetUserByUserID(user, true));
                    }

                    game.Users = userEntities;

                    List<int> GameDecks = new List<int>();
                    using (MySqlConnection conn = GetConnection())
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@GameID", game.GameID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GameDecks.Add(Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "DeckID")));
                            }
                        }
                    }

                    var deckDAL = new DatabaseLogicLayer.Decks(this.sqlConnection);

                    var deckEntities = new List<DataEntities.Deck>();

                    foreach (var deck in GameDecks)
                    {
                        deckEntities.Add(deckDAL.GetDeckByID(deck));
                    }

                    game.Decks = deckEntities;

                    //Game Options
                    if (isActive == true)
                    {
                        sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID AND Active = 1;";
                    }
                    else if (isActive == false)
                    {
                        sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID AND Active = 0;";
                    }
                    else
                    {
                        sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID;";
                    }

                    List<int> GameOptionsPicked = new List<int>();
                    using (MySqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@GameID", game.GameID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GameOptionsPicked.Add(Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "UserID")));
                            }
                        }
                    }

                    var gameOptionDAL = new DatabaseLogicLayer.GameOptions(this.sqlConnection);

                    var gameOptionEntities = new List<DataEntities.GameOption>();

                    foreach (var optionID in GameOptionsPicked)
                    {
                        gameOptionEntities.Add(gameOptionDAL.GetGameOptionByGameOptionID(optionID, true));
                    }

                    game.GameOptions = gameOptionEntities;

                    return ret;
                }
            }
            else if (ret != null)
            {
                return ret;
            }

            return ret;
        }

        public int? SaveGameState(DataEntities.GameState p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (p.GameStateID == null) //Overwrite userid to userauth for now TODO
                {
                    sql = @"INSERT INTO `game_state` (`GameID`, `Data`) VALUES (@GameID, @Data);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `game_state` SET GameID = @GameID, Data = @Data WHERE GameStateID = @GameStateID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@GameStateID", p.GameStateID);
                cmd.Parameters.AddWithValue("@GameID", p.GameID);
                cmd.Parameters.AddWithValue("@Data", p.Data);

                if (p.GameStateID == null) //Get new id number
                {
                    return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }
                else //Return id if worked
                {
                    cmd.ExecuteScalar();

                    return p.GameStateID;
                }
            }
        }

        public long? SaveGameUser(DataEntities.GameUser p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                List<DataEntities.GameUser> ret = new List<DataEntities.GameUser>();

                string sql = "";

                sql = "SELECT * FROM game_user WHERE GameID = @GameID;";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@GameID", p.GameID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(ConvertMySQLToEntityGU(reader));
                    }
                }

                var userAlreadySavedCheck = ret.Where(x => x.UserID == p.UserID).ToList();

                if (userAlreadySavedCheck == null || userAlreadySavedCheck.Any() == false)
                {
                    sql = "";

                    if (p.GameUserID == null) //Overwrite userid to userauth for now TODO
                    {
                        sql = @"INSERT INTO `game_user` (`GameUserID`, `GameID`, `UserID`, `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy`, `Active`) VALUES (NULL, @GameID, @UserID, @CreatedDate, @ModifiedDate, @CreatedBy, @ModifiedBy, @Active);
                            SELECT LAST_INSERT_ID();";
                    }
                    else
                    {
                        sql = @"UPDATE `game_user` SET GameID = @GameID, UserID = @UserID, CreatedDate = @CreatedDate,  ModifiedDate = @ModifiedDate, CreatedBy = @CreatedBy, ModifiedBy = @ModifiedBy, Active = @Active WHERE GameUserID = @GameUserID;";
                    }

                    cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@GameUserID", p.GameUserID);
                    cmd.Parameters.AddWithValue("@GameID", p.GameID);
                    cmd.Parameters.AddWithValue("@UserID", p.UserID);
                    cmd.Parameters.AddWithValue("@CreatedDate", p.CreatedDate);
                    cmd.Parameters.AddWithValue("@ModifiedDate", p.ModifiedDate);
                    cmd.Parameters.AddWithValue("@CreatedBy", p.CreatedBy);
                    cmd.Parameters.AddWithValue("@ModifiedBy", p.ModifiedBy);
                    cmd.Parameters.AddWithValue("@Active", p.Active);

                    if (p.GameUserID == null) //Get new id number
                    {
                        return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                    }
                    else //Return id if worked
                    {
                        cmd.ExecuteScalar();

                        return p.GameUserID;
                    }
                }

                return null;
            }
        }

        public long? SaveGameOptionsPicked(DataEntities.GameOptionPicked p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                List<DataEntities.GameUser> ret = new List<DataEntities.GameUser>();

                string sql = "";

                sql = "SELECT * FROM game_optionpicked WHERE GameID = @GameID;";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@GameID", p.GameID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(ConvertMySQLToEntityGU(reader));
                    }
                }

                var optionSavedBeforeCheck = ret.Where(x => x.UserID == p.GameOptionID).ToList();

                if (optionSavedBeforeCheck == null || optionSavedBeforeCheck.Any() == false)
                {
                    sql = "";

                    if (p.GameOptionPickedID == null) //Overwrite userid to userauth for now TODO
                    {
                        sql = @"INSERT INTO `game_optionpicked` (`GameOptionPickedID`, `GameID`, `GameOptionID`, `Active`) VALUES (NULL, @GameID, @GameOptionID, @Active);
                            SELECT LAST_INSERT_ID();";
                    }
                    else
                    {
                        sql = @"UPDATE `game_optionpicked` SET GameID = @GameID, GameOptionID = @GameOptionID, Active = @Active WHERE GameOptionPickedID = @GameOptionPickedID;";
                    }

                    cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@GameOptionPickedID", p.GameOptionPickedID);
                    cmd.Parameters.AddWithValue("@GameID", p.GameID);
                    cmd.Parameters.AddWithValue("@GameOptionID", p.GameOptionID);
                    cmd.Parameters.AddWithValue("@Active", p.Active);

                    if (p.GameOptionPickedID == null) //Get new id number
                    {
                        return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                    }
                    else //Return id if worked
                    {
                        cmd.ExecuteScalar();

                        return p.GameOptionPickedID;
                    }
                }

                return null;
            }
        }

        public long? SaveGameDeck(DataEntities.GameDeck p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                List<DataEntities.GameDeck> ret = new List<DataEntities.GameDeck>();

                string sql = "";

                sql = "SELECT * FROM game_deck WHERE GameID = @GameID;";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@GameID", p.GameID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(ConvertMySQLToEntityGD(reader));
                    }
                }

                var userAlreadySavedCheck = ret.Where(x => x.DeckID == p.DeckID).ToList();

                if (userAlreadySavedCheck == null || userAlreadySavedCheck.Any() == false)
                {
                    sql = "";

                    if (p.GameDeckID == null)
                    {
                        sql = @"INSERT INTO `game_deck` (`GameDeckID`, `GameID`, `DeckID`, `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy`, `Active`) VALUES (NULL, @GameID, @DeckID, @CreatedDate, @ModifiedDate, @CreatedBy, @ModifiedBy, @Active);
                            SELECT LAST_INSERT_ID();";
                    }
                    else
                    {
                        sql = @"UPDATE `game_user` SET GameID = @GameID, DeckID = @DeckID, CreatedDate = @CreatedDate,  ModifiedDate = @ModifiedDate, CreatedBy = @CreatedBy, ModifiedBy = @ModifiedBy, Active = @Active WHERE GameDeckID = @GameDeckID;";
                    }

                    cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@GameDeckID", p.GameDeckID);
                    cmd.Parameters.AddWithValue("@GameID", p.GameID);
                    cmd.Parameters.AddWithValue("@DeckID", p.DeckID);
                    cmd.Parameters.AddWithValue("@CreatedDate", p.CreatedDate);
                    cmd.Parameters.AddWithValue("@ModifiedDate", p.ModifiedDate);
                    cmd.Parameters.AddWithValue("@CreatedBy", p.CreatedBy);
                    cmd.Parameters.AddWithValue("@ModifiedBy", p.ModifiedBy);
                    cmd.Parameters.AddWithValue("@Active", p.Active);

                    if (p.GameDeckID == null) //Get new id number
                    {
                        return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                    }
                    else //Return id if worked
                    {
                        cmd.ExecuteScalar();

                        return p.GameDeckID;
                    }
                }

                return null;
            }
        }

        public int? SaveGame(DataEntities.Game p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (p.GameID == null) //Overwrite userid to userauth for now TODO
                {
                    sql = @"INSERT INTO `game` (`GameID`, `Name`, `IsPublic`, `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy`, `Active`) VALUES (NULL, @Name, @IsPublic, @CreatedDate, @ModifiedDate, @CreatedBy, @ModifiedBy, @Active);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `game` SET Name = @Name, IsPublic = @IsPublic, CreatedDate = @CreatedDate,  ModifiedDate = @ModifiedDate, CreatedBy = @CreatedBy, ModifiedBy = @ModifiedBy, Active = @Active WHERE GameID = @GameID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@GameID", p.GameID);
                cmd.Parameters.AddWithValue("@Name", p.Name);
                cmd.Parameters.AddWithValue("@IsPublic", p.IsPublic);
                cmd.Parameters.AddWithValue("@CreatedDate", p.CreatedDate);
                cmd.Parameters.AddWithValue("@ModifiedDate", p.ModifiedDate);
                cmd.Parameters.AddWithValue("@CreatedBy", p.CreatedBy);
                cmd.Parameters.AddWithValue("@ModifiedBy", p.ModifiedBy);
                cmd.Parameters.AddWithValue("@Active", p.Active);

                int? ret = null;

                if (p.GameID == null) //Get new id number
                {
                    ret = cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }
                else //Return id if worked
                {
                    cmd.ExecuteScalar();

                    ret = p.GameID;
                }

                if (ret.HasValue)
                {
                    conn.Close();

                    var gameDecks = GetGamesDecksByGameID(ret.GetValueOrDefault(), true);

                    conn.Open();

                    sql = @"delete from game_deck where GameID = @ID";

                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ID", ret.GetValueOrDefault());

                    cmd.ExecuteNonQuery();

                    conn.Close();

                    foreach (var deck in p.Decks)
                    {
                        var isInSaveDeck = gameDecks.Where(x => x.DeckID == deck.DeckID).ToList().Any();

                        var gameUser = new DataEntities.GameDeck();
                        if (isInSaveDeck) //Already in game
                        {
                            gameUser.GameID = ret.GetValueOrDefault();
                            gameUser.DeckID = deck.DeckID.GetValueOrDefault();
                            gameUser.ModifiedDate = DateTime.UtcNow;
                            gameUser.ModifiedBy = p.ModifiedBy;
                            gameUser.Active = true;
                        }
                        else //Not in game
                        {
                            gameUser.GameID = ret.GetValueOrDefault();
                            gameUser.DeckID = deck.DeckID.GetValueOrDefault();
                            gameUser.CreatedDate = DateTime.UtcNow;
                            gameUser.CreatedBy = p.ModifiedBy;
                            gameUser.ModifiedDate = DateTime.UtcNow;
                            gameUser.ModifiedBy = p.ModifiedBy;
                            gameUser.Active = true;
                        }

                        SaveGameDeck(gameUser);
                    }

                    conn.Close();

                    var gameGameOptionPicks = GetGameOptionsPickedByGameID(ret.GetValueOrDefault(), true);

                    conn.Open();

                    sql = @"delete from game_optionpicked where GameID = @ID";

                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ID", ret.GetValueOrDefault());

                    cmd.ExecuteNonQuery();

                    conn.Close();

                    foreach (var option in p.GameOptions)
                    {
                        var isInGameOptions = gameGameOptionPicks.Where(x => x.GameOptionID == option.GameOptionID).ToList().Any(); //TODO make game option have ability for text field and save value to db

                        var gameOption = new DataEntities.GameOptionPicked();
                        if (isInGameOptions) //Already in game
                        {
                            gameOption.GameID = ret.GetValueOrDefault();
                            gameOption.GameOptionID = option.GameOptionID.GetValueOrDefault();
                            gameOption.Active = true;
                        }
                        else //Not in game
                        {
                            gameOption.GameID = ret.GetValueOrDefault();
                            gameOption.GameOptionID = option.GameOptionID.GetValueOrDefault();
                            gameOption.Active = true;
                        }

                        SaveGameOptionsPicked(gameOption);
                    }

                    foreach (var user in p.Users)
                    {
                        var gameUser = new DataEntities.GameUser();
                        gameUser.GameID = ret.GetValueOrDefault();
                        gameUser.UserID = user.UserID.GetValueOrDefault();
                        gameUser.ModifiedDate = DateTime.UtcNow;
                        gameUser.ModifiedBy = p.ModifiedBy;
                        gameUser.Active = user.Active;

                        SaveGameUser(gameUser);
                    }
                }

                return ret;
            }
        }

        internal DataEntities.Game ConvertMySQLToEntity(MySqlDataReader reader)
        {
            DataEntities.Game p = new DataEntities.Game();

            p.GameID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameID"));
            p.Name = DBUtilities.ReturnSafeString(reader, "Name");
            p.IsPublic = Convert.ToBoolean(DBUtilities.ReturnBoolean(reader, "IsPublic"));
            p.CreatedDate = DBUtilities.ReturnSafeDateTime(reader, "CreatedDate").Value;
            p.ModifiedDate = DBUtilities.ReturnSafeDateTime(reader, "ModifiedDate").Value;
            p.CreatedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CreatedBy"));
            p.ModifiedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "ModifiedBy"));
            p.Active = Convert.ToBoolean(DBUtilities.ReturnBoolean(reader, "Active"));

            return p;
        }

        internal DataEntities.GameUser ConvertMySQLToEntityGU(MySqlDataReader reader)
        {
            DataEntities.GameUser p = new DataEntities.GameUser();

            p.GameUserID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameUserID"));
            p.GameID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameID"));
            p.UserID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "UserID"));
            p.CreatedDate = DBUtilities.ReturnSafeDateTime(reader, "CreatedDate").Value;
            p.ModifiedDate = DBUtilities.ReturnSafeDateTime(reader, "ModifiedDate").Value;
            p.CreatedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CreatedBy"));
            p.ModifiedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "ModifiedBy"));
            p.Active = Convert.ToBoolean(DBUtilities.ReturnBoolean(reader, "Active"));

            return p;
        }

        internal DataEntities.GameDeck ConvertMySQLToEntityGD(MySqlDataReader reader)
        {
            DataEntities.GameDeck p = new DataEntities.GameDeck();

            p.GameDeckID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameDeckID"));
            p.GameID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameID"));
            p.DeckID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "DeckID"));
            p.CreatedDate = DBUtilities.ReturnSafeDateTime(reader, "CreatedDate").Value;
            p.ModifiedDate = DBUtilities.ReturnSafeDateTime(reader, "ModifiedDate").Value;
            p.CreatedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CreatedBy"));
            p.ModifiedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "ModifiedBy"));
            p.Active = Convert.ToBoolean(DBUtilities.ReturnBoolean(reader, "Active"));

            return p;
        }

        internal DataEntities.GameOptionPicked ConvertMySQLToEntityGOP(MySqlDataReader reader)
        {
            DataEntities.GameOptionPicked p = new DataEntities.GameOptionPicked();

            p.GameOptionPickedID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameOptionPickedID"));
            p.GameID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameID"));
            p.GameOptionID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameOptionID"));
            p.Active = Convert.ToBoolean(DBUtilities.ReturnBoolean(reader, "Active"));

            return p;
        }

        internal DataEntities.GameState ConvertMySQLToEntityGS(MySqlDataReader reader)
        {
            DataEntities.GameState p = new DataEntities.GameState();

            p.GameStateID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameStateID"));
            p.GameID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameID"));
            
            var data = DBUtilities.ReturnSafeString(reader, "Data");

            var entity = p.GameStateFromString(data);
            entity.GameID = p.GameID;
            entity.GameStateID = p.GameStateID;

            return entity;
        }
    }
}
