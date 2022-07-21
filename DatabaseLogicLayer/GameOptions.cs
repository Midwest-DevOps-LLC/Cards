using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseLogicLayer
{
    public class GameOptions : DBManager
    {
        #region Initializer

        string ConnectionString { get; set; }

        MySqlConnection sqlConnection { get; set; }

        public GameOptions(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public GameOptions(MySqlConnection sqlConnection)
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

        public DataEntities.GameOption GetGameOptionByGameOptionID(int gameOptionID, bool? isActive)
        {
            var ret = new DataEntities.GameOption();

            string sql = "";

            if (isActive == true)
            {
                sql = "SELECT * FROM game_option WHERE GameOptionID = @GameOptionID AND Active = 1;";
            }
            else if (isActive == false)
            {
                sql = "SELECT * FROM game_option WHERE GameOptionID = @GameOptionID AND Active = 0;";
            }
            else
            {
                sql = "SELECT * FROM game_option WHERE GameOptionID = @GameOptionID;";
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@GameOptionID", gameOptionID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret = ConvertMySQLToEntity(reader);
                    }
                }
            }

            return ret;
        }

        public List<DataEntities.GameOption> GetAllGameOptions(bool? isActive)
        {
            List<DataEntities.GameOption> ret = new List<DataEntities.GameOption>();

            string sql = "";

            if (isActive == true)
            {
                sql = "SELECT * FROM game_option WHERE Active = 1;";
            }
            else if (isActive == false)
            {
                sql = "SELECT * FROM game_option WHERE Active = 0;";
            }
            else
            {
                sql = "SELECT * FROM game_option;";
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

            return ret;
        }

        public long? SaveGameOption(DataEntities.GameOption p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (p.GameOptionID == null) //Overwrite userid to userauth for now TODO
                {
                    sql = @"INSERT INTO `game_option` (`Name`, `Active`) VALUES (@Name, @Active);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `game_option` SET Name = @Name, Active = @Active WHERE GameOptionID = @GameOptionID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@GameUserID", p.GameOptionID);
                cmd.Parameters.AddWithValue("@Name", p.Name);
                cmd.Parameters.AddWithValue("@Active", p.Active);

                if (p.GameOptionID == null) //Get new id number
                {
                    return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }
                else //Return id if worked
                {
                    cmd.ExecuteScalar();

                    return p.GameOptionID;
                }
            }
        }

        internal DataEntities.GameOption ConvertMySQLToEntity(MySqlDataReader reader)
        {
            DataEntities.GameOption p = new DataEntities.GameOption();

            p.GameOptionID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "GameOptionID"));
            p.Name = DBUtilities.ReturnSafeString(reader, "Name");
            p.Active = Convert.ToBoolean(DBUtilities.ReturnBoolean(reader, "Active"));

            return p;
        }
    }
}
