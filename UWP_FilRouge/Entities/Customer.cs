using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Entities
{
    public class Customer : User
    {
        private long idCustomer;
        private string town;
        private int postalCode;
        private string address;
        private int loyaltyPoints;
        private string phone;
        private string email;
        /// private List<Order> orders;
        private Shop shop;

        public long IdCustomer
        {
            get { return idCustomer; }
            set
            {
                idCustomer = value;
                OnPropertyChanged("IdCustomer");
            }
        }

        public string Town
        {
            get { return town; }
            set
            {
                town = value;
                OnPropertyChanged("Town");
            }
        }
        public int PostalCode
        {
            get { return postalCode; }
            set
            {
                postalCode = value;
                OnPropertyChanged("PostalCode");
            }
        }
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }
        public int LoyaltyPoints
        {
            get { return loyaltyPoints; }
            set
            {
                loyaltyPoints = value;
                OnPropertyChanged("LoyaltyPoints");
            }
        }
        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                OnPropertyChanged("Phone");
            }
        }
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged("Email");
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

        public event PropertyChangedEventHandler PropertyChanged;

        public override object Copy()
        {
            Customer customer = new Customer();
            customer.idCustomer = this.idCustomer;
            /// customer.IdCustomer = this.IdCustomer;
            customer.Address = this.Address;
            customer.Email = this.Email;
            customer.LoyaltyPoints = this.LoyaltyPoints;
            customer.Phone = this.Phone;
            customer.PostalCode = this.PostalCode;
            /// customer.Shop = this.Shop;
            if (this.Shop != null)
            {
                customer.Shop = this.Shop.Copy() as Shop;
            }
            customer.Town = this.Town;

            return customer;
        }

        public override void CopyFrom(object obj)
        {
            Customer customer = obj as Customer;
            customer.idCustomer = this.idCustomer;
            /// customer.IdCustomer = this.IdCustomer;
            customer.Address = this.Address;
            customer.Email = this.Email;
            customer.LoyaltyPoints = this.LoyaltyPoints;
            customer.Phone = this.Phone;
            customer.PostalCode = this.PostalCode;
            /// customer.Shop = this.Shop;
            if (customer.Shop != null)
            {
                this.Shop = customer.Shop;
            }
            customer.Town = this.Town;
        }

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