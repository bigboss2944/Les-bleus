using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Seller : User
    {
        #region Attributes
        private long idSeller;
        private string password;
        private List<Order> orders;
        private Shop shop;
        private Role role;
        #endregion

        #region properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdSeller 
        { 
            get => idSeller; 
            set => idSeller = value; 
        }

        public Role Role
        {
            get { return role; }
            set { role = value; }
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

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

        #region constructors
        public Seller()
        {
            this.Orders = new List<Order>();
        }
        #endregion

        #region Functions
        public String ToString()
        {
            return this.idSeller + " " + this.password + " " + this.orders + " " + this.shop + " " + this.role;
        }
        #endregion
    }
}