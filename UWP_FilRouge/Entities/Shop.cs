using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Entities
{
    public class Shop : EntityBase

    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public override object Copy()
        {
            Shop shop = new Shop();
            shop.IdShop = this.IdShop;
            shop.Adress = this.Adress;
            shop.Email = this.Email;
            shop.Nameshop = this.Nameshop;
            shop.Phone = this.Phone;
            shop.Postalcode = this.Postalcode;
            shop.Town = this.Town;
            shop.Website = this.Website;
            /*if (this.Role != null)
            {
                shop.Role = this.Role.Copy() as Role;
            }*/

            return shop;
        }

        public override void CopyFrom(object obj)
        {
            Shop shop = obj as Shop;
            this.IdShop = shop.Id;
            this.Adress = shop.Adress;
            this.Email = shop.Email;
            this.Nameshop = shop.Nameshop;
            this.Phone = shop.Phone;
            this.Postalcode = shop.Postalcode;
            this.Town = shop.Town;
            this.Website = shop.Website;
            /*if (user.Role != null)
            {
                this.Role = user.Role;
            }*/
        }

        private long idShop;
        private string town;
        private int postalCode;
        private string adress;
        private string nameShop;
        private string phone;
        private string email;
        private string website;
        /*private List<Order> orders;
        private List<Seller> sellers;
        private List<Customer> customers;*/

        public long IdShop
        {
            get { return idShop; }
            set { 
                idShop = value;
                OnPropertyChanged("IdShop");
            
            }
        }

        public string Town
        {
            get { return town; }
            set { 
                town = value;
                OnPropertyChanged("Town");
            }
        }

        public int Postalcode
        {
            get { return postalCode; }
            set { 
                postalCode = value;
                OnPropertyChanged("PostalCode");
         
            }
        }

        public string Adress
        {
            get { return adress; }
            set { 
                adress = value;
                OnPropertyChanged("Adress");
            }
        }

        public string Nameshop
        {
            get { return nameShop; }
            set { 
                nameShop = value;
                OnPropertyChanged("NameShop");
            }
        }

        public string Phone
        {
            get { return phone; }
            set { 
                phone = value;
                OnPropertyChanged("Phone");
            }
        }

        public string Email
        {
            get { return email; }
            set { 
                email = value;
                OnPropertyChanged("Email");
            }
        }

        public string Website
        {
            get { return website; }
            set { 
                website = value;
                OnPropertyChanged("Website");
            }
        }

        /*public List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }


        public List<Seller> Sellers
        {
            get { return sellers; }
            set { sellers = value; }
        }

        public List<Customer> Customers
        {
            get { return customers; }
            set { customers = value; }
        } */



    }
}
