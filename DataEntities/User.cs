using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class User : BaseEntity
    {
        public int? UserID
        {
            get; set;
        }

        public int UserAuthID
        {
            get; set;
        }

        public string Username
        {
            get; set;
        }

        public string ChatName
        {
            get; set;
        }
    }
}
