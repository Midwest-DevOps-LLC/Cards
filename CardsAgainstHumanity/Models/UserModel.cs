using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsAgainstHumanity.Models
{
    public class UserModel : BaseModel
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

        public UserModel()
        {

        }

        public UserModel(DataEntities.User p)
        {
            this.UserID = p.UserID;
            this.UserAuthID = p.UserAuthID;
            this.ChatName = p.ChatName;
            this.Username = p.Username;
            this.Active = p.Active;
            this.CreatedBy = p.CreatedBy;
            this.CreatedDate = p.CreatedDate;
            this.ModifiedBy = p.ModifiedBy;
            this.ModifiedDate = p.ModifiedDate;
        }

        public DataEntities.User ConvertToEntity()
        {
            DataEntities.User p = new DataEntities.User();

            p.UserID = this.UserID;
            p.UserAuthID = this.UserAuthID;
            p.ChatName = this.ChatName;
            p.Username = this.Username;
            p.Active = this.Active;
            p.CreatedBy = this.CreatedBy;
            p.CreatedDate = this.CreatedDate;
            p.ModifiedBy = this.ModifiedBy;
            p.ModifiedDate = this.ModifiedDate;

            return p;
        }
    }
}
