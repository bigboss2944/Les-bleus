using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Entities
{
    public class Seller : EntityBase
    {

        private long idSeller;
        private string category;
        private string password;
        /*private List<Order> orders;*/
        private Shop shop;

        public long IdSeller
        {
            get { return idSeller; }
            set
            {
                idSeller = value;
                OnPropertyChanged("IdSeller");
            }
        }
        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                OnPropertyChanged("Category");
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        /*public List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }*/

        public Shop Shop
        {
            get { return shop; }
            set
            {
                shop = value;
                OnPropertyChanged("Shop");
            }
        }


        public override object Copy()
        {
            Seller seller = new Seller();
            seller.Id = this.Id;
            seller.Category = this.Category;
            seller.Password = this.Password;
            /// seller.Shop = this.Shop;
            if (this.Shop != null)
            {
                seller.Shop = this.Shop.Copy() as Shop;
            }



            return seller;
           
        }

        public override void CopyFrom(object obj)
        {
            Seller seller = obj as Seller;
            this.Id = seller.Id;
            this.Password = seller.Password;
            ///this.Shop = seller.Shop;
            this.Category = seller.Category;
            if (seller.Shop != null)
            {
                this.Shop = seller.Shop;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
