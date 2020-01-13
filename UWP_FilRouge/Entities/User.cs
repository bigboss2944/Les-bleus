using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class User
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
        #endregion

    }
}

