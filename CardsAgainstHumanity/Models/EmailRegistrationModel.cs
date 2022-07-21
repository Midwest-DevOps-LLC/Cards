using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class EmailRegistrationModel
    {
        public int? EmailRegistrationID
        {
            get; set;
        }

        public string UUID
        {
            get; set;
        }

        public int UserID
        {
            get; set;
        }

        public bool Active
        {
            get; set;
        }
    }
}
