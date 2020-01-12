using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP_NET_FilRouge.Models
{
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
        private List<Order> orders;
        private Shop shop;
        #endregion

        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdCustomer { get => idCustomer; set => idCustomer = value; }

        public string Town { get => town; set => town = value; }
        public int PostalCode { get => postalCode; set => postalCode = value; }
        public string Address { get => address; set => address = value; }
        public int LoyaltyPoints { get => loyaltyPoints; set => loyaltyPoints = value; }
        public string Phone { get => phone; set => phone = value; }
        public string Email { get => email; set => email = value; }
        public List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }
        public Shop Shop
        {
            get { return shop; }
            set { shop = value; }
        }
        #endregion

        #region Constructors
        public Customer() : base()
        {
            this.Orders = new List<Order>();
        }
        #endregion     
    }
}
