using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class WhiteCardSaveModel
    {
        public int? WhiteCardID
        {
            get; set;
        }

        public string Text
        {
            get; set;
        }

        public int CreatedBy
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public WhiteCardSaveModel()
        {

        }

        public WhiteCardSaveModel(DataEntities.WhiteCard e)
        {
            this.WhiteCardID = e.WhiteCardID;
            this.Text = e.Text;
            this.CreatedBy = e.CreatedBy;
        }
    }
}
