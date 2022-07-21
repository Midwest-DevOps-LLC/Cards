using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class GameUser : BaseEntity
    {
        public int? GameUserID { get; set; }
        public int GameID { get; set; }
        public int UserID { get; set; }
    }
}
