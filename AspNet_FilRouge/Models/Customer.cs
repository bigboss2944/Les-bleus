using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet_FilRouge.Models
{
    public class Customer : User
    {
        #region Attributes
        private string town;
        private int postalCode;
        private string address;
        private int loyaltyPoints;
        private string phone;
        private string email;
        private Shop shop;
        private List<Order> orders;
        private string gender;
        #endregion

        #region Properties
       

        public string Town { get => town; set => town = value; }
        public int PostalCode { get => postalCode; set => postalCode = value; }
        public string Address { get => address; set => address = value; }
        public int LoyaltyPoints { get => loyaltyPoints; set => loyaltyPoints = value; }
        public string Phone { get => phone; set => phone = value; }
        public string Email { get => email; set => email = value; }

        public Shop Shop
        {
            get { return shop; }
            set { shop = value; }
        }

        public List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        #endregion

        #region Constructors
        public Customer() : base()
        {
            this.Orders = new List<Order>();
        }
        #endregion    

        #region Functions
        public String ToString()
        {
            return this.town + " " + this.postalCode + " " + this.address + " " + this.loyaltyPoints + " " + this.phone + " " + this.email + " " + this.gender + " " + this.shop + " " + this.orders;
        }
        #endregion

    }
}
