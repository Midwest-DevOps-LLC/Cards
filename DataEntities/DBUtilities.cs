using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public static class DBUtilities
    {
        public static string ReturnSafeString(MySqlDataReader reader, String columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return reader.GetString(columnName);
            }
            else
            {
                return null;
            }
        }

        public static int? ReturnSafeInt(MySqlDataReader reader, String columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return reader.GetString(columnName).ConvertToNullableInt();
            }
            else
            {
                return null;
            }
        }

        public static double? ReturnSafeDouble(MySqlDataReader reader, String columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return reader.GetString(columnName).ConvertToNullableDouble();
            }
            else
            {
                return null;
            }
        }

        public static DateTime? ReturnSafeDateTime(MySqlDataReader reader, String columnName)
        {
            int index = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(index))
            {
                return reader.GetString(columnName).ConvertToNullableDateTime();
            }
            else
            {
                return null;
            }
        }

        public static bool ReturnBoolean(MySqlDataReader reader, String columnName)
        {
            return reader.GetString(columnName) == "1" ? true : false;
        }

        public static DateTime ReturnDateTime(MySqlDataReader reader, String columnName)
        {
            return DateTime.Parse(reader.GetString(columnName));
        }
    }
}
