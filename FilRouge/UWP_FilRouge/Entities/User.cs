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
        private String firstname;

        public String Firstname
        {
            get { return firstname; }
            set
            {
                firstname = value;
                OnPropertyChanged("Firstname");
            }
        }

        private String lastname;

        public String Lastname
        {
            get { return lastname; }
            set
            {
                lastname = value;
                OnPropertyChanged("Lastname");
            }
        }

        private Seller seller;

        public Seller Seller
        {
            get { return seller; }
            set
            {
                seller = value;
                OnPropertyChanged("Seller");
            }
        }

        public override object Copy()
        {
            User user = new User();
            user.Id = this.Id;
            user.Firstname = this.Firstname;
            user.Lastname = this.Lastname;
            if (this.Seller != null)
            {
                user.Seller = this.Seller.Copy() as Seller;
            }

            return user;
        }

        public override void CopyFrom(object obj)
        {
            User user = obj as User;
            this.Id = user.Id;
            this.Firstname = user.Firstname;
            this.Lastname = user.Lastname;
            if (user.Seller != null)
            {
                this.Seller = user.Seller;
            }
        }

    }
}

