using System;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    /// <summary>
    /// test
    /// </summary>
    public class BaseEntity
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
