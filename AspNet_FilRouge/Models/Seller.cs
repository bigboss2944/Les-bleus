using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNet_FilRouge.Models
{
    public class Seller : User
    {
        #region Attributes
        private List<Order> orders;
        private Shop shop;
        
        #endregion

        #region properties
        

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
            return " ";
        }
        #endregion
    }
}