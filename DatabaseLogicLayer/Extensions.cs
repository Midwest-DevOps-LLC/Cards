using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseLogicLayer
{
    public static class Extensions
    {
        public static int? ConvertToNullableInt(this string i)
        {
            if (String.IsNullOrEmpty(i))
            {
                return null;
            }

            return Convert.ToInt32(i);
        }

        public static double? ConvertToNullableDouble(this string i)
        {
            if (String.IsNullOrEmpty(i))
            {
                return null;
            }

            return Convert.ToDouble(i);
        }

        public static DateTime? ConvertToNullableDateTime(this string i)
        {
            if (String.IsNullOrEmpty(i))
            {
                return null;
            }

            return Convert.ToDateTime(i);
        }
    }
}
