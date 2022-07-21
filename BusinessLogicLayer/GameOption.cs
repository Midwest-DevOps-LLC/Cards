using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class GameOption : BLLManager, IDisposable
    {
        public GameOption(string connectionString, ref DataEntities.ValidationEvent validationEvent)
        {
            this.ConnectionString = connectionString;
            this.validationEvent = validationEvent;
        }

        public GameOption(MySqlConnection sqlConnection, ref DataEntities.ValidationEvent validationEvent)
        {
            this.sqlConnection = sqlConnection;
            this.validationEvent = validationEvent;
        }

        public DataEntities.GameOption GetGameOptionByGameOptionID(int userID, bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.GameOptions personDLL = new DatabaseLogicLayer.GameOptions(GetConnection());

                return personDLL.GetGameOptionByGameOptionID(userID, isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }

        public List<DataEntities.GameOption> GetAllGameOptions(bool? isActive)
        {
            try
            {
                DatabaseLogicLayer.GameOptions personDLL = new DatabaseLogicLayer.GameOptions(GetConnection());

                return personDLL.GetAllGameOptions(isActive);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return new List<DataEntities.GameOption>();
        }

        public long? SaveUser(DataEntities.GameOption person)
        {
            try
            {
                DatabaseLogicLayer.GameOptions personDLL = new DatabaseLogicLayer.GameOptions(GetConnection());

                return personDLL.SaveGameOption(person);
            }
            catch (Exception e)
            {
                Logging.SaveLog(new Log() { time = DateTime.UtcNow, exception = e });
            }

            return null;
        }
    }
}
