using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModelLight.FichiersASPNET;

namespace UWP_FilRouge.Views.ViewModelLight.Entities
{
    public class Order: EntityBase
    {

        private int quantity;

        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        private Seller seller;

        public Seller Seller
        {
            get { return seller; }
            set
            {
                seller = value;
                OnPropertyChanged("Lastname");
            }
        }

        private Role role;

        public Role Role
        {
            get { return role; }
            set
            {
                role = value;
                OnPropertyChanged("Role");
            }
        }

        public override object Copy()
        {
            Order user = new Order();
            user.Id = this.Id;
            user.Quantity = this.Quantity;
            user.Seller = this.Seller;
            if (this.Role != null)
            {
                user.Role = this.Role.Copy() as Role;
            }

            return user;
        }

        public override void CopyFrom(object obj)
        {
            Order order = obj as Order;
            this.Id = order.Id;
            this.Quantity = order.Quantity;
            this.Seller = order.Seller;
            if (order.Role != null)
            {
                this.Role = order.Role;
            }
        }

    }
}
