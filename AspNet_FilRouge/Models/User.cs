using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet_FilRouge.Models
{
    public class User : IdentityUser
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
        public User() : base()
        {

        }
        #endregion

    }
}

