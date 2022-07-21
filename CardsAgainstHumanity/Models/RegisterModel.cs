using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class RegisterModel
    {
        public string Username
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string Application
        {
            get; set;
        }

        public string Password
        {
            get; set;
        }

        public string RetypePassword
        {
            get; set;
        }
    }
}
