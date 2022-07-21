using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class Users : BLLManager, IDisposable
    {
        public Users(string connectionString, ref DataEntities.ValidationEvent validationEvent)
        {
            this.ConnectionString = connectionString;
            this.validationEvent = validationEvent;
        }

        public Users(MySqlConnection sqlConnection, ref DataEntities.ValidationEvent validationEvent)
        {
            this.sqlConnection = sqlConnection;
            this.validationEvent = validationEvent;
        }

        public DataEntities.User GetUserByUserID(int userID, bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.Users personDLL = new DatabaseLogicLayer.Users(GetConnection());

                return personDLL.GetUserByUserID(userID, isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public DataEntities.User GetUserByUsername(string username, bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.Users personDLL = new DatabaseLogicLayer.Users(GetConnection());

                return personDLL.GetUserByUsername(username, isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public DataEntities.User GetUserByUserAuthID(int userAuthID, bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.Users personDLL = new DatabaseLogicLayer.Users(GetConnection());

                return personDLL.GetUserByUserAuthID(userAuthID, isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public List<DataEntities.User> GetAllUsers(bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.Users personDLL = new DatabaseLogicLayer.Users(GetConnection());

                return personDLL.GetAllUsers(isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.User>();
        }

        public long? SaveUser(DataEntities.User person)
        {
            try
            {
                DatabaseLogicLayer.Users personDLL = new DatabaseLogicLayer.Users(GetConnection());

                return personDLL.SaveUser(person);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }
    }
}
