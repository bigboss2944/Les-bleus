using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Entities;

namespace UWP_FilRouge
{
    public class User : EntityBase
    {
        #region Attributs
        protected string firstName;
        protected string password;
        private int id;

        #endregion

        #region Properties
        public string FirstName { get => firstName; set => firstName = value; }
        public string Password { get => password; set => password = value; }


        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        #endregion

        #region Constructors
        public User()
        {

        }
        #endregion


        public override object Copy()
        {
            User user = new User();
            user.Id = this.Id;
            user.FirstName = this.FirstName;
            user.Password = this.Password;
            

            return user;
        }

        public override void CopyFrom(object obj)
        {
            User user = obj as User;
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.Password = user.Password;
            
            
        }
    }
}

