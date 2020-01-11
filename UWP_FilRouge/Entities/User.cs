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

