using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseLogicLayer
{
    public class Users : DBManager
    {
        #region Initializer

        string ConnectionString { get; set; }

        MySqlConnection sqlConnection { get; set; }

        public Users(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public Users(MySqlConnection sqlConnection)
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

        public DataEntities.User GetUserByUserID(int userID, bool? isActive)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (isActive == true)
                {
                    sql = "SELECT * FROM user WHERE UserID = @UserID AND Active = 1;";
                }
                else if (isActive == false)
                {
                    sql = "SELECT * FROM user WHERE UserID = @UserID AND Active = 0;";
                }
                else
                {
                    sql = "SELECT * FROM user WHERE UserID = @UserID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@UserID", userID);
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

        public DataEntities.User GetUserByUsername(string username, bool? isActive)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (isActive == true)
                {
                    sql = "SELECT * FROM user WHERE Username LIKE @Username AND Active = 1;";
                }
                else if (isActive == false)
                {
                    sql = "SELECT * FROM user WHERE Username LIKE @Username AND Active = 0;";
                }
                else
                {
                    sql = "SELECT * FROM user WHERE Username LIKE @Username;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Username", username.ToUpper());
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

        public DataEntities.User GetUserByUserAuthID(int userAuthID, bool? isActive)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (isActive == true)
                {
                    sql = "SELECT * FROM user WHERE UserAuthID = @UserAuthID AND Active = 1;";
                }
                else if (isActive == false)
                {
                    sql = "SELECT * FROM user WHERE UserAuthID = @UserAuthID AND Active = 0;";
                }
                else
                {
                    sql = "SELECT * FROM user WHERE UserAuthID = @UserAuthID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@UserAuthID", userAuthID);
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

        public List<DataEntities.User> GetAllUsers(bool? isActive)
        {
            List<DataEntities.User> p = new List<DataEntities.User>();

            string sql = "";

            if (isActive == true)
            {
                sql = "SELECT * FROM user WHERE Active = 1;";
            }
            else if (isActive == false)
            {
                sql = "SELECT * FROM user WHERE Active = 0;";
            }
            else
            {
                sql = "SELECT * FROM user;";
            }

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
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

        public long? SaveUser(DataEntities.User p)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                string sql = "";

                if (p.UserID == null) //Overwrite userid to userauth for now TODO
                {
                    sql = @"INSERT INTO `user` (`UserID`, `UserAuthID`, `Username`, `ChatName`, `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy`, `Active`) VALUES (NULL, @UserAuthID, @Username, @ChatName, @CreatedDate, @ModifiedDate, @CreatedBy, @ModifiedBy, @Active);
                            SELECT LAST_INSERT_ID();";
                }
                else
                {
                    sql = @"UPDATE `user` SET UserAuthID = @UserAuthID, Username = @Username, ChatName = @ChatName, CreatedDate = @CreatedDate,  ModifiedDate = @ModifiedDate, CreatedBy = @CreatedBy, ModifiedBy = @ModifiedBy, Active = @Active WHERE UserID = @UserID;";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@UserID", p.UserID);
                cmd.Parameters.AddWithValue("@UserAuthID", p.UserAuthID);
                cmd.Parameters.AddWithValue("@Username", p.Username);
                cmd.Parameters.AddWithValue("@ChatName", p.ChatName);
                cmd.Parameters.AddWithValue("@CreatedDate", p.CreatedDate);
                cmd.Parameters.AddWithValue("@ModifiedDate", p.ModifiedDate);
                cmd.Parameters.AddWithValue("@CreatedBy", p.CreatedBy);
                cmd.Parameters.AddWithValue("@ModifiedBy", p.ModifiedBy);
                cmd.Parameters.AddWithValue("@Active", p.Active);

                if (p.UserID == null) //Get new id number
                {
                    return cmd.ExecuteScalar().ToString().ConvertToNullableInt();
                }
                else //Return id if worked
                {
                    cmd.ExecuteScalar();

                    return p.UserID;
                }
            }
        }

        internal DataEntities.User ConvertMySQLToEntity(MySqlDataReader reader)
        {
            DataEntities.User p = new DataEntities.User();

            p.UserID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "UserID"));
            p.UserAuthID = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "UserAuthID"));
            p.Username = DBUtilities.ReturnSafeString(reader, "Username");
            p.ChatName = DBUtilities.ReturnSafeString(reader, "ChatName");
            p.CreatedDate = DBUtilities.ReturnSafeDateTime(reader, "CreatedDate").Value;
            p.ModifiedDate = DBUtilities.ReturnSafeDateTime(reader, "ModifiedDate").Value;
            p.CreatedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "CreatedBy"));
            p.ModifiedBy = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "ModifiedBy"));
            p.Active = Convert.ToBoolean(DBUtilities.ReturnBoolean(reader, "Active"));

            return p;
        }
    }
}
