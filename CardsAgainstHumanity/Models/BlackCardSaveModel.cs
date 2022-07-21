using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class BlackCardSaveModel
    {
        public int? BlackCardID
        {
            get; set;
        }

        public string Text
        {
            get; set;
        }

        public int Pick
        {
            get; set;
        }

        public int Draw
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

        public BlackCardSaveModel()
        {

        }

        public BlackCardSaveModel(DataEntities.BlackCard e)
        {
            this.BlackCardID = e.BlackCardID;
            this.Text = e.Text;
            this.Pick = e.Pick;
            this.Draw = e.Draw;
            this.CreatedBy = e.CreatedBy;
        }
    }
}
