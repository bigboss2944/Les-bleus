using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Entities;

namespace Entities
{
    public class User : EntityBase
    {
        #region Attributs
        protected string lastname;
        protected string firstname;
        #endregion

        #region Properties
        public string Lastname { get => lastname; set => lastname = value; }
        public string Firstname { get => firstname; set => firstname = value; }
        #endregion

        #region Constructors
        public User()
        {


        }

        public override object Copy()
        {
            User user = new User();
            user.Lastname = this.lastname;
            user.Firstname = this.firstname;

            return user;
        }

        public override void CopyFrom(object obj)
        {
            User user = obj as User;
            user.Lastname = this.lastname;
            user.Firstname = this.firstname;
        }
        #endregion

    }
}

