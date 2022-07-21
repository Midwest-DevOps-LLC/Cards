using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public class CardBattle
    {
        public long? CardBattleID { get; set; }
        public int? UserID { get; set; }
        public WhiteCard Card1 { get; set; }
        public WhiteCard Card2 { get; set; }
        public WhiteCard WinningCard { get; set; }

        public class CardBattleWinner
        {
            public int ID { get; set; }
            public int WinnerID { get; set; }
            public bool? Success { get; set; }
            public string Message { get; set; }
        }

        public class CardBattleIndex
        {
            public List<CardBattleRank> WhiteCards { get; set; }

            public class CardBattleRank
            {
                public int Rank { get; set; }
                public string CardText { get; set; }

                public static CardBattleRank ConvertMySQLToEntity(MySqlDataReader reader)
                {
                    CardBattleRank p = new CardBattleRank();

                    p.Rank = Convert.ToInt32(DBUtilities.ReturnSafeInt(reader, "Rank"));
                    p.CardText = DBUtilities.ReturnSafeString(reader, "Text");

                    return p;
                }
            }
        }
    }
}
