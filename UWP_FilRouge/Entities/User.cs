using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge
{
    public class User
    {
        #region Attributs
        protected string firstName;
        protected string password;
        #endregion

        #region Properties
        public string FirstName { get => firstName; set => firstName = value; }
        public string Password { get => password; set => password = value; }
        #endregion

        #region Constructors
        public User()
        {

        }
        #endregion

    }
}

