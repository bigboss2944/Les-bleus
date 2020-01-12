using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AspNet_FilRouge.Models
{
    public class Order
    {
        #region Attributs
        private long idOrder;
        private List<Bicycle> bicycles;
        private Seller seller;
        private Customer customer;
        private DateTime date;
        private Shop shop;
        private string payMode;
        private float discount;
        private bool useLoyaltyPoint;
        private float tax;
        private float shippingCost;
        #endregion

        #region Properties 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdOrder
        {
            get { return idOrder; }
            set { idOrder = value; }
        }

        public List<Bicycle> Bicycles
        {
            get { return bicycles; }
            set { bicycles = value; }
        }

        public Seller Seller
        {
            get { return seller; }
            set { seller = value; }
        }

        public Customer Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public Shop Shop
        {
            get { return shop; }
            set { shop = value; }
        }

        public string PayMode
        {
            get { return payMode; }
            set { payMode = value; }
        }

        public float Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        public bool UseLoyaltyPoint
        {
            get { return useLoyaltyPoint; }
            set { useLoyaltyPoint = value; }
        }

        public float Tax
        {
            get { return tax; }
            set { tax = value; }
        }

        public float ShippingCost
        {
            get { return shippingCost; }
            set { shippingCost = value; }
        }

        #endregion

        #region Constructors
        public Order()
        {
            this.Bicycles = new List<Bicycle>();
        }
        #endregion

        #region Functions
        #endregion
    }
}