﻿using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge
{
    [Table("Order")]
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
        private List<Bicycle> listBicycle;
        private Seller subSeller;
        private Customer subCustomer;
        private Shop subShop;
        #endregion

        #region Properties 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdOrder
        {
            get { return idOrder; }
            set { idOrder = value; }
        }

        [ManyToMany(typeof(BicycleOrder))]
        public List<Bicycle> ListBicycle { get => listBicycle; set => listBicycle = value; }

        [ManyToOne]
        public Customer SubCustomer { get => subCustomer; set => subCustomer = value; }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        [ManyToOne]
        public Shop SubShop { get => subShop; set => subShop = value; }

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
            //this.Bicycles = new List<Bicycle>();
        }
        #endregion

        #region Functions
        public String ToString()
        {
            return this.idOrder + " " + this.bicycles + " " + this.shop + " " + this.seller + " " + this.customer + " " + this.date + " " + this.payMode + " " + this.discount + " " + this.useLoyaltyPoint + " " + this.tax + " " + this.shippingCost;
        }
        #endregion

    }
}