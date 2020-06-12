using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge
{
    [Table("Customer")]
    public class Customer : User
    {
        #region Attributes
        private long idCustomer;
        private string town;
        private int postalCode;
        private string address;
        private int loyaltyPoints;
        private string phone;
        private string email;
        private Shop shop;
        private List<Order> orders;
        private string gender;
        private Customer subCustomer;
        private Shop subShop;
        #endregion

        #region Properties



        public string Town { get => town; set => town = value; }
        public int PostalCode { get => postalCode; set => postalCode = value; }
        public string Address { get => address; set => address = value; }
        public int LoyaltyPoints { get => loyaltyPoints; set => loyaltyPoints = value; }
        public string Phone { get => phone; set => phone = value; }
        public string Email { get => email; set => email = value; }

        [ManyToOne]
        public Customer SubCustomer { get => subCustomer; set => subCustomer = value; }

        [ManyToOne]
        public Shop SubShop { get => subShop; set => subShop = value; }

        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        #endregion

        #region Constructors
        public Customer() : base()
        {
            //this.Orders = new List<Order>();
        }
        #endregion    

        #region Functions
        public String ToString()
        {
            return this.idCustomer + " " + this.town + " " + this.postalCode + " " + this.address + " " + this.loyaltyPoints + " " + this.phone + " " + this.email + " " + this.gender + " " + this.shop + " " + this.orders;
        }
        #endregion

    }
}
