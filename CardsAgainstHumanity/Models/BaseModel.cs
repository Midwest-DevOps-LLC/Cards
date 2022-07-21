using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class BaseModel
    {
        public bool Active
        {
            get; set;
        }

        public int CreatedBy
        {
            get; set;
        }

        public DateTime CreatedDate
        {
            get; set;
        }

        public int ModifiedBy
        {
            get; set;
        }

        public DateTime ModifiedDate
        {
            get; set;
        }
    }
}
