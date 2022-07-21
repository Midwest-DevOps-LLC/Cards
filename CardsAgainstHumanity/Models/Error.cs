using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class Error
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
