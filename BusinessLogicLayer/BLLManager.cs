using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public class BLLManager
    {
        internal string ConnectionString { get; set; }

        internal MySqlConnection sqlConnection { get; set; }

        internal DataEntities.ValidationEvent validationEvent { get; set; }

        public MySqlConnection GetConnection()
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

        public void Dispose()
        {
            if (sqlConnection != null)
                sqlConnection.Dispose();
        }
    }
}
