using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet_FilRouge.Models
{
    public class User
    {
        #region Attributs
        protected string lastName;
        protected string firstName;
        #endregion

        #region Properties
        public string LastName { get => lastName; set => lastName = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        #endregion

        #region Constructors
        public User()
        {

        }
        #endregion

    }
}